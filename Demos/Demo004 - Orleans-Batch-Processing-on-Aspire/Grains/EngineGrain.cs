using BatchProcessing.Abstractions.Configuration;
using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using BatchProcessing.Domain.Models;
using BatchProcessing.Grains.Services;
using BatchProcessing.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orleans.Placement;

namespace BatchProcessing.Grains;

/// <summary>
/// The EngineGrain class is responsible for simulating the processing of records.
/// It coordinates with the EngineGovernorGrain to ensure that the number of concurrently running engines does not exceed the allowed capacity.
/// </summary>
[RegionDelegatingPlacement<RandomPlacement>]
internal class EngineGrain(ContextFactory contextFactory, IOptions<EngineConfig> config, ILogger<EngineGrain> logger) : Grain, IEngineGrain
{
    private readonly CancellationTokenSource _shutdownCancellation = new();
    private Task? _backgroundTask;

    // Worker Count would be set through configuration and drives how many workers we have processing records
    private readonly int _workerCount = config.Value.WorkerCount;

    private int _recordCount;
    private int _recordsProcessed;
    private string? _regionScope;
    private readonly DateTime _createdOn = DateTime.UtcNow;

    private AnalysisStatus _status = AnalysisStatus.NotStarted;

    /// <summary>
    /// Initiates the analysis process with the specified number of records to simulate.
    /// </summary>
    /// <param name="recordsToSimulate">The number of records to simulate processing.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RunAnalysis(int recordsToSimulate)
    {
        if (_backgroundTask is null or { IsCompleted: true })
        {
            _regionScope = RequestContext.Get(RegionDelegatingPlacement.RegionHintKey) as string;
            _regionScope ??= this.GetGrainId().GetKeyExtension();

            await CreateRecords(recordsToSimulate);

            _status = AnalysisStatus.NotStarted;
            _backgroundTask = Task.Factory.StartNew(ProcessBackgroundTask, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current).Unwrap();
        }
    }

    private async Task CreateRecords(int recordsToSimulate)
    {
        using var context = contextFactory.Create();

        var batchProcess = new BatchProcess
        {
            Id = this.GetPrimaryKey(),
            Status = BatchProcessStatus.Created,
            CreatedOn = DateTime.UtcNow
        };
        context.BatchProcesses.Add(batchProcess);

        await context.SaveChangesAsync();

        await CreateAndPersistRecords(recordsToSimulate, batchProcess, contextFactory);
    }

    private static async Task CreateAndPersistRecords(int recordsToSimulate, BatchProcess batchProcess, ContextFactory contextFactory)
    {
        var items = BogusService.Generate(batchProcess.Id, recordsToSimulate);

        var batchSize = 100;
        var tasks = items.Chunk(batchSize).Select(batch => PersistRecords(batch, contextFactory));

        await Task.WhenAll(tasks);
    }

    private static async Task PersistRecords(IEnumerable<BatchProcessItem> items, ContextFactory contextFactory)
    {
        using var context = contextFactory.Create();
        await context.BulkInsert(items);
    }

    /// <summary>
    /// Retrieves the current status of the engine.
    /// </summary>
    /// <returns>A Task containing the EngineStatusRecord with the current status.</returns>
    public Task<EngineStatusRecord> GetStatus()
    {
        var status = new EngineStatusRecord(this.GetPrimaryKey(), _status, _regionScope, _recordCount, _recordsProcessed, _createdOn);
        return Task.FromResult(status);
    }

    /// <summary>
    /// Processes the background task for record analysis.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task ProcessBackgroundTask()
    {
        await Task.Yield();

        _recordCount = await GetRecordCount();

        var workerTasks = new List<Task>();

        var governor = GrainFactory.GetGrain<IEngineGovernorGrain>(0);

        while (!_shutdownCancellation.IsCancellationRequested)
        {
            // Check with the Governor to see if we can start
            if (_status == AnalysisStatus.NotStarted)
            {
                var response = await governor.TryStartEngine(new EngineStatusRecord(this.GetPrimaryKey(), _status, _regionScope, _recordCount, _recordsProcessed, _createdOn));

                if (!response.Success)
                {
                    logger.LogWarning("{ProcessId} - Unable to start processing: {Reason}", this.GetPrimaryKey(), response.Reason);
                    await Task.Delay(1000);
                    continue;
                }

                await SetStarted(governor);
            }

            var batch = await GetBatch(_workerCount);

            if (batch.Count == 0)
            {
                break;
            }

            logger.LogInformation("{ProcessId} - Processing batch of {Count} records", this.GetPrimaryKey(), batch.Count);

            int i = 0;

            foreach (var item in batch)
            {
                var worker = GrainFactory.GetGrain<IEngineWorkerGrain>($"{this.GetPrimaryKey()}-{i}");
                workerTasks.Add(worker.DoWork(item.Id));
                i++;
            }

            RequestContext.Set(RegionDelegatingPlacement.RegionHintKey, _regionScope);

            await Task.WhenAll(workerTasks);

            _recordsProcessed += batch.Count;

        }

        await SetCompleted(governor);
    }

    private async Task SetCompleted(IEngineGovernorGrain governor)
    {
        try
        {
            using var context = contextFactory.Create();

            var key = this.GetPrimaryKey();

            var batchProcess = await context.BatchProcesses.FirstOrDefaultAsync(bp => bp.Id == key)
                ?? throw new InvalidOperationException("Batch Process not found");

            var aggregateResult = await GenerateAggregateResult(batchProcess);

            batchProcess.AggregateResult = aggregateResult;
            batchProcess.CompletedOn = DateTime.UtcNow;
            batchProcess.Status = BatchProcessStatus.Completed;

            await context.SaveChangesAsync();

            _status = AnalysisStatus.Completed;

            await governor.UpdateStatus(new EngineStatusRecord(this.GetPrimaryKey(), _status, _regionScope, _recordCount, _recordsProcessed, _createdOn));

            logger.LogInformation("{ProcessId} - Analysis completed", this.GetPrimaryKey());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ProcessId} - Error completing analysis", this.GetPrimaryKey());
            throw;
        }
    }

    private async Task SetStarted(IEngineGovernorGrain governor)
    {
        try
        {
            using var context = contextFactory.Create();

            var key = this.GetPrimaryKey();

            var batchProcess = await context.BatchProcesses.FirstOrDefaultAsync(bp => bp.Id == key) 
                ?? throw new InvalidOperationException("Batch Process not found");
            batchProcess.Status = BatchProcessStatus.Running;

            await context.SaveChangesAsync();

            _status = AnalysisStatus.InProgress;

            // Update Governor with status
            await governor.UpdateStatus(new EngineStatusRecord(this.GetPrimaryKey(), _status, _regionScope, _recordCount, _recordsProcessed, _createdOn));

            logger.LogInformation("{ProcessId} - Analysis in progress", this.GetPrimaryKey());

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{ProcessId} - Error starting analysis", this.GetPrimaryKey());
            throw;
        }
    }

    /// <summary>
    /// Retrieves the total number of records to be processed.
    /// </summary>
    /// <returns>A Task containing the number of records to be processed.</returns>
    private async Task<int> GetRecordCount()
    {
        var processId = this.GetPrimaryKey();
        var context = contextFactory.Create();

        return await context.BatchProcessItems.CountAsync(bpi => bpi.BatchProcessId == processId);
    }

    /// <summary>
    /// Retrieves a batch of records to process.
    /// </summary>
    /// <param name="workerCount"></param>
    /// <returns>A Task containing a list of records to process.</returns>
    private async Task<List<AnalysisRecord>> GetBatch(int workerCount)
    {
        using var context = contextFactory.Create();

        var batch = await context.BatchProcessItems
            .Where(bpi => bpi.BatchProcessId == this.GetPrimaryKey() && bpi.Status == BatchProcessItemStatus.Created)
            .Select(bpi => bpi.Id)
            .Take(workerCount)
            .ToListAsync();

        return [.. batch.Select(bpi => new AnalysisRecord(bpi))];
    }

    /// <summary>
    /// Handles the deactivation of the grain, ensuring that the background task is properly canceled and awaited.
    /// </summary>
    /// <param name="reason">The reason for deactivation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _shutdownCancellation.Cancel();
        if (_backgroundTask is { } task && !task.IsCompleted)
        {
            // Wait for the background task to complete, but don't wait indefinitely.
            await task.WaitAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Generates the aggregate result for a given batch process.
    /// </summary>
    /// <param name="batchProcess">The batch process for which to generate the aggregate result.</param>
    /// <returns>The generated aggregate result or null if no result is available.</returns>
    private async Task<BatchProcessAggregateResult?> GenerateAggregateResult(BatchProcess batchProcess)
    {
        using var context = contextFactory.Create();

        var analysisTimestamp = DateTime.UtcNow;

        var records = await context.BatchProcessItems
            .Select(item => new
            {
                item.BatchProcessId,
                Age = DateTime.Now.Year - item.Person.DateOfBirth.Year,
                item.Person.HouseholdSize,
                item.Person.NumberOfDependents,
                item.Person.MaritalStatus
            })
            .Where(i => i.BatchProcessId == batchProcess.Id)
            .ToListAsync();

        var results = records
            .GroupBy(i => i.BatchProcessId)
            .Select(grouping => new
            {
                BatchProcessId = batchProcess.Id,
                AverageAge = grouping.Average(item => item.Age),
                AverageDependents = grouping.Average(item => item.NumberOfDependents),
                AverageHouseholdSize = grouping.Average(item => item.HouseholdSize),
                AverageMaritalStatus = grouping.GroupBy(item => item.MaritalStatus)
                    .Select(msGroup => new MaritalStatusRecordAverage
                    {
                        MaritalStatus = msGroup.Key,
                        AverageCount = msGroup.Average(item => item.Age)
                    }).ToList()
            })
            .FirstOrDefault();

        if (results is null) return null;

        return new BatchProcessAggregateResult(
            batchProcess.Id,
            analysisTimestamp,
            results.AverageAge,
            results.AverageDependents,
            results.AverageHouseholdSize,
            results.AverageMaritalStatus
        );
    }
}
