using BatchProcessing.Shared;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace BatchProcessing.Domain.Models;

public class BatchProcess
{
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public BatchProcessStatus Status { get; set; }

    public BatchProcessAggregateResult? AggregateResult { get; set; }
}