using BatchProcessing.Shared;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BatchProcessing.Domain.Models;

public class BatchProcessItem
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    public Guid BatchProcessId { get; set; }

    public BatchProcessItemStatus Status { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public required Person Person { get; set; }

    public BatchProcessItemAnalysisResult? AnalysisResult { get; set; }
}