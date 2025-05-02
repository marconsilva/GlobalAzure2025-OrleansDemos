namespace BatchProcessing.Domain.Models;

/// <summary>
/// Represents a person with a first name, last name, date of birth, and address.
/// </summary>
public class Person
{
    /// <summary>
    /// Gets or sets the first name of the person.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the person.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the date of birth of the person.
    /// </summary>
    public required DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Gets or sets the address of the person.
    /// </summary>
    public required Address Address { get; set; }

    /// <summary>
    /// Gets or sets the marital status of the person.
    /// </summary>
    public required string MaritalStatus { get; set; }

    /// <summary>
    /// Gets or sets the number of dependents of the person.
    /// </summary>
    public required int NumberOfDependents { get; set; }

    /// <summary>
    /// Gets or sets the household size of the person.
    /// </summary>
    public required int HouseholdSize { get; set; }
}
