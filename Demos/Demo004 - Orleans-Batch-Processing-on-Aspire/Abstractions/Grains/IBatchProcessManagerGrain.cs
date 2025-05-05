namespace BatchProcessing.Abstractions.Grains;
using Orleans;

[Alias("BatchProcessing.Abstractions.Grains.IBatchProcessManagerGrain")]
public interface IBatchProcessManagerGrain : IGrainWithIntegerKey
{
    [Alias("GetBatchProcesses")]
    Task<IEnumerable<BatchProcessRecord>> GetBatchProcesses();

    [Alias("GetBatchProcess")]
    Task<BatchProcessRecord?> GetBatchProcess(Guid engineId);
}