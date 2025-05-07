using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using BatchProcessing.Domain.Models;
using BatchProcessing.Shared;
using Microsoft.Extensions.Logging;
using Orleans.Placement;
using Orleans.Runtime.Placement;

namespace BatchProcessing.Grains;

[RegionDelegatingPlacement<RandomPlacement>]
internal class EngineWorkerGrain(ContextFactory contextFactory, ILogger<EngineWorkerGrain> logger) : Grain, IEngineWorkerGrain
{
    private static readonly Random Random = new();

    public async Task DoWork(Guid id)
    {
        var regionScope = RequestContext.Get(RegionDelegatingPlacement.RegionHintKey) as string;
        logger.LogInformation("{Id} in region {RegionScope} is processing item {ItemId}", this.GetGrainId(), regionScope, id);

        try
        {
            await using var context = contextFactory.Create();

            var item = await context.BatchProcessItems.FindAsync(id);

            if (item is not null)
            {
                item.Status = BatchProcessItemStatus.Running;

                await context.SaveChangesAsync();

                // TODO: This is where we'll do some "analysis work" on the item
                // Simulate some work delay between 125 and 475 milliseconds
                await Task.Delay(Random.Next(125, 475));

                item.AnalysisResult = GenerateAnalysisResult(item);
                item.Status = BatchProcessItemStatus.Completed;

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing item {Id}", id);
        }
        finally
        {
            // Deactivate the grain as soon as the work is over to free up resources
            DeactivateOnIdle();
        }
    }

    /// <summary>
    /// Generates analysis results for a given BatchProcessItem.
    /// </summary>
    /// <param name="item">The BatchProcessItem to analyze.</param>
    /// <returns>The generated BatchProcessItemAnalysisResult object.</returns>
    private static BatchProcessItemAnalysisResult GenerateAnalysisResult(BatchProcessItem item)
    {
        var age = DateTime.Now.Year - item.Person.DateOfBirth.Year;
        return new BatchProcessItemAnalysisResult(DateTime.UtcNow, age, item.Person.MaritalStatus,
            item.Person.NumberOfDependents, item.Person.HouseholdSize);
    }
}