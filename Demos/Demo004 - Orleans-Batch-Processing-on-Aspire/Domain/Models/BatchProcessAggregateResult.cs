using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BatchProcessing.Domain.Models;

public class BatchProcessAggregateResult
{
    [BsonRepresentation(BsonType.String)]
    public Guid BatchProcessId { get; set; }

    public DateTime AnalysisTimestamp { get; set; }

    public double AverageAge { get; set; }

    public double AverageDependents { get; set; }

    public double AverageHouseholdSize { get; set; }

    public IEnumerable<MaritalStatusRecordAverage> MaritalStatusCounts { get; set; } = new List<MaritalStatusRecordAverage>();

    public BatchProcessAggregateResult()
    {
    }

    public BatchProcessAggregateResult(Guid batchProcessId, DateTime analysisTimestamp, double averageAge, double averageDependents, double averageHouseholdSize, ICollection<MaritalStatusRecordAverage> maritalStatusCounts)
    {
        BatchProcessId = batchProcessId;
        AnalysisTimestamp = analysisTimestamp;
        AverageAge = averageAge;
        AverageDependents = averageDependents;
        AverageHouseholdSize = averageHouseholdSize;
        MaritalStatusCounts = maritalStatusCounts;
    }
}