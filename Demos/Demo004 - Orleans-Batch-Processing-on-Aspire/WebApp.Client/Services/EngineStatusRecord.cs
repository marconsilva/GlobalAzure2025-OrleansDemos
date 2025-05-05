namespace BatchProcessing.WebApp.Services;

public record EngineStatusRecord
{
    public Guid Id { get; set; }

    public AnalysisStatus Status { get; set; }

    public string? RegionScope { get; set; }

    public int RecordCount { get; set; }

    public int RecordsProcessed { get; set; } = 0;

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public BatchProcessResut? AggregateResult { get; set; }
}