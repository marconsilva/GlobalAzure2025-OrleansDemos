using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using BatchProcessing.Shared;
using Microsoft.EntityFrameworkCore;
using Orleans.Concurrency;

namespace BatchProcessing.Grains;

[StatelessWorker]
public class BatchProcessManagerGrain(ContextFactory contextFactory) : IBatchProcessManagerGrain
{
    public async Task<IEnumerable<BatchProcessRecord>> GetBatchProcesses()
    {
        await using var context = contextFactory.Create();

        var batchProcessRecords = await context.BatchProcesses
            .Select(x => new BatchProcessProjection(x.Id, x.CreatedOn, x.CompletedOn, x.Status)).ToListAsync();

        return await SetRecordCountsAsync(batchProcessRecords);
    }

    private async Task<IEnumerable<BatchProcessRecord>> SetRecordCountsAsync(List<BatchProcessProjection> batchProcessRecords)
    {
        await using var context = contextFactory.Create();

        var results = new List<BatchProcessRecord>();

        foreach (var batchProcessRecord in batchProcessRecords)
        {
            var recordCount = await context.BatchProcessItems.CountAsync(x => x.BatchProcessId == batchProcessRecord.Id);
            results.Add(new BatchProcessRecord(batchProcessRecord.Id, batchProcessRecord.CreatedOn, batchProcessRecord.CompletedOn, batchProcessRecord.Status, recordCount));
        }

        return results;
    }

    [ReadOnly]
    public async Task<BatchProcessRecord?> GetBatchProcess(Guid engineId)
    {
        await using var context = contextFactory.Create();

        var fullList= await context.BatchProcesses.Select(bp => new
        {
            bp.Id,
            bp.CreatedOn,
            bp.CompletedOn,
            bp.Status,
            AggregateResult = bp.AggregateResult == null ? null : new {
                bp.Id,
                bp.AggregateResult.AnalysisTimestamp,
                bp.AggregateResult.AverageAge,
                bp.AggregateResult.AverageDependents,
                bp.AggregateResult.AverageHouseholdSize,
                //bp.AggregateResult.MaritalStatusCounts.Select(x =>
                //    new MaritalStatusRecordAverageRecord(x.MaritalStatus, x.AverageCount)))
                MaritalStatusCounts = bp.AggregateResult.MaritalStatusCounts.Select(x =>
                    new
                    {
                        x.MaritalStatus,
                        x.AverageCount
                    })
            }
        })
        .ToListAsync();

        var batchProcess = fullList.FirstOrDefault(x => x.Id == engineId);

        if (batchProcess is null)
        {
            return null;
        }

        var aggregateResult = batchProcess.AggregateResult == null ? null : new BatchProcessAggregateResultRecord(
            batchProcess.Id,
            batchProcess.AggregateResult.AnalysisTimestamp,
            batchProcess.AggregateResult.AverageAge,
            batchProcess.AggregateResult.AverageDependents,
            batchProcess.AggregateResult.AverageHouseholdSize,
            batchProcess.AggregateResult.MaritalStatusCounts.Select(x =>
                new MaritalStatusRecordAverageRecord(x.MaritalStatus, x.AverageCount)).ToList());

        var recordCount = await context.BatchProcessItems.CountAsync(x => x.BatchProcessId == engineId);

        return new BatchProcessRecord(batchProcess.Id, batchProcess.CreatedOn, batchProcess.CompletedOn, batchProcess.Status, recordCount, aggregateResult);
    }

    private record BatchProcessProjection(
        Guid Id,
        DateTime CreatedOn,
        DateTime? CompletedOn,
        BatchProcessStatus Status);
}