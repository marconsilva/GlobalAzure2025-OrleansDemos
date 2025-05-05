namespace BatchProcessing.Abstractions.Grains;
using Orleans;

[Alias("BatchProcessing.Abstractions.Grains.IEngineWorkerGrain")]
public interface IEngineWorkerGrain : IGrainWithStringKey
{
    [Alias("DoWork")]
    Task DoWork(Guid id);
}
