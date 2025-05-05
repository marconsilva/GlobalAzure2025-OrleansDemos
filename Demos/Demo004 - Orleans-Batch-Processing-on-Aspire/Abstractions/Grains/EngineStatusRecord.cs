using BatchProcessing.Shared;

namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record EngineStatusRecord(
    Guid Id,
    AnalysisStatus Status,
    string? RegionScope,
    int RecordCount,
    int RecordsProcessed,
    DateTime CreatedOn);