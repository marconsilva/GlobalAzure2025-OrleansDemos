namespace BatchProcessing.Abstractions.Grains;
using Orleans;

[Alias("BatchProcessing.Abstractions.Grains.IEngineGovernorGrain")]
public interface IEngineGovernorGrain : IGrainWithIntegerKey
{
    [Alias("TryStartEngine")]
    Task<TryStartResponse> TryStartEngine(EngineStatusRecord engineStatus);

    [Alias("UpdateStatus")]
    Task UpdateStatus(EngineStatusRecord engineStatus); 
}