namespace BatchProcessing.WebApp.Services;

public record BatchProcessResut
{
    public DateTime AnalysisTimestamp { get; set; }

    public double AverageAge { get; set; }

    public double AverageDependents { get; set; }

    public double AverageHouseholdSize { get; set; }

    public required IEnumerable<MaritalStatusAverage> MaritalStatusCounts { get; set; }
}
