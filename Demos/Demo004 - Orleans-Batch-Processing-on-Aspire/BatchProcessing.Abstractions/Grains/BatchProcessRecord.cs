using BatchProcessing.Shared;

namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record BatchProcessRecord(
    Guid Id,
    DateTime CreatedOn,
    DateTime? CompletedOn,
    BatchProcessStatus Status,
    int RecordCount = 0,
    BatchProcessAggregateResultRecord? AggregateResult = null);