using Orleans;

namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record BatchProcessAggregateResultRecord(
    Guid BatchProcessId,
    DateTime AnalysisTimestamp,
    double AverageAge,
    double AverageDependents,
    double AverageHouseholdSize,
    List<MaritalStatusRecordAverageRecord> MaritalStatusCounts);