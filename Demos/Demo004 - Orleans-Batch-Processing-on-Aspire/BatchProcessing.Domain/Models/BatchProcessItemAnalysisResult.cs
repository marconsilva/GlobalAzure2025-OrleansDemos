namespace BatchProcessing.Domain.Models;

public record BatchProcessItemAnalysisResult(DateTime AnalysisTimestamp, int Age, string MaritalStatus, int NumberOfDependents, int HouseholdSize);