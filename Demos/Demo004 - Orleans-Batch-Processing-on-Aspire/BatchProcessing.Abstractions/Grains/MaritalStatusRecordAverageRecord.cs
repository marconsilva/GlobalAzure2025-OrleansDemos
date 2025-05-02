namespace BatchProcessing.Abstractions.Grains;
using Orleans;

[GenerateSerializer]
public record MaritalStatusRecordAverageRecord(string MaritalStatus, double AverageCount);