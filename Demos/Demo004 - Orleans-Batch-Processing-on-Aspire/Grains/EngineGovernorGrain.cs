using BatchProcessing.Abstractions.Configuration;
using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Shared;
using Microsoft.Extensions.Options;

namespace BatchProcessing.Grains;

/// <summary>
/// The EngineGovernorGrain class is responsible for managing the number of engines that can run concurrently.
/// It ensures that the number of engines in the "InProgress" state does not exceed the maximum capacity.
///
/// NOTE: This class could also help manage scaling out the number of engines based on the current load.
/// </summary>
internal class EngineGovernorGrain(IOptions<EngineConfig> config) : IEngineGovernorGrain
{
    // List to maintain the state of each engine
    private readonly List<EngineGovernorStateRecord> _queue = [];

    // Maximum number of engines that can run concurrently
    private readonly int _maxCapacity = config.Value.MaxActiveEngine;

    /// <summary>
    /// Attempts to start an engine. If the engine is already in the state list, it updates the last updated time.
    /// If the number of engines in the "InProgress" state is less than the maximum capacity, it sets the engine state to "InProgress".
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be started.</param>
    /// <returns>A TryStartResponse indicating whether the engine was successfully started or not.</returns>
    public Task<TryStartResponse> TryStartEngine(EngineStatusRecord engineStatus)
    {
        // Check to see if this engine is already in the state list
        if (_queue.All(x => x.EngineStatus.Id != engineStatus.Id))
        {
            _queue.Add(new EngineGovernorStateRecord(engineStatus, engineStatus.CreatedOn, DateTime.UtcNow));
        }

        // Check if we have capacity based on what is running
        if (_queue.Count(x => x.EngineStatus.Status == AnalysisStatus.InProgress
            && string.Equals(x.EngineStatus.RegionScope, engineStatus.RegionScope, StringComparison.OrdinalIgnoreCase)) >= _maxCapacity)
        {
            SetLastUpdated(engineStatus);
            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Engine at capacity"));
        }

        // check to see if this is the next available item to run based on created time
        var nextAvailable = GetNextExpectedFromQueue(engineStatus.RegionScope);

        if (nextAvailable is not null && nextAvailable.EngineStatus.Id != engineStatus.Id)
        {
            SetLastUpdated(engineStatus);
            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Engine not next available"));
        }

        // Set the engine state to "InProgress"
        var stateInProgress = _queue.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
        stateInProgress?.SetState(AnalysisStatus.InProgress);

        return Task.FromResult(new TryStartResponse(engineStatus.Id, true, string.Empty));
    }

    /// <summary>
    /// Sets the last updated time for the specified engine status.
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be updated.</param>
    private void SetLastUpdated(EngineStatusRecord engineStatus)
    {
        var state = _queue.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
        state?.SetLastUpdated(DateTime.UtcNow);
    }

    /// <summary>
    /// Gets the next expected engine from the queue based on the created time.
    /// </summary>
    /// <returns>The next expected EngineGovernorStateRecord or null if none are available.</returns>
    private EngineGovernorStateRecord? GetNextExpectedFromQueue(string? regionScope) => _queue
        .Where(x => x.EngineStatus.Status == AnalysisStatus.NotStarted && string.Equals(x.EngineStatus.RegionScope, regionScope, StringComparison.OrdinalIgnoreCase))
        .MinBy(x => x.CreatedOn);

    /// <summary>
    /// Updates the status of an engine in the state list.
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be updated.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public Task UpdateStatus(EngineStatusRecord engineStatus)
    {
        var state = _queue.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
        state?.SetState(engineStatus.Status);
        return Task.CompletedTask;
    }
}
