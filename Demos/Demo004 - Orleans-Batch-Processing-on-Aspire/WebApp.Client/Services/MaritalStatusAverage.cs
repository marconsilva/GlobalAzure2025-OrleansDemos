namespace BatchProcessing.WebApp.Services;

public record MaritalStatusAverage
{
    public required string MaritalStatus { get; set; }

    public double AverageCount { get; set; }
}