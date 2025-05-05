namespace BatchProcessing.Domain.Models;

public class MaritalStatusRecordAverage
{
    public required string MaritalStatus { get; set; }

    public double AverageCount { get; set; }
}