namespace BatchProcessing.Abstractions.Grains;
using Orleans;

[GenerateSerializer]
public record TryStartResponse(Guid Id, bool Success, string Reason);