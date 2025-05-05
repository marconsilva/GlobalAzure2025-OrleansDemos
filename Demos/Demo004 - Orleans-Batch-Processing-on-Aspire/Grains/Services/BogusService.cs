using BatchProcessing.Domain.Models;
using BatchProcessing.Shared;
using Bogus;
using Person = BatchProcessing.Domain.Models.Person;

namespace BatchProcessing.Grains.Services;

/// <summary>
/// A static class that provides methods for generating fake data using the Bogus library.
/// </summary>
internal static class BogusService
{
    /// <summary>
    /// Generates a list of fake BatchProcessItem objects.
    /// </summary>
    /// <param name="batchId">The ID of the batch process.</param>
    /// <param name="numberOfRecords">The number of records to generate.</param>
    /// <returns>A list of generated BatchProcessItem objects.</returns>
    public static List<BatchProcessItem> Generate(Guid batchId, int numberOfRecords)
    {
        var batchProcessItemFaker = new Faker<BatchProcessItem>()
            .RuleFor(b => b.Id, f => f.Random.Guid())
            .RuleFor(b => b.BatchProcessId, f => batchId)
            .RuleFor(b => b.Status, f => BatchProcessItemStatus.Created)
            .RuleFor(b => b.CreatedOnUtc, f => DateTime.UtcNow)
            .RuleFor(b => b.Person, f => GeneratePerson());

        return batchProcessItemFaker.Generate(numberOfRecords);
    }

    /// <summary>
    /// Generates a fake Person object.
    /// </summary>
    /// <returns>The generated Person object.</returns>
    public static Person GeneratePerson()
    {
        var personFaker = new Faker<Person>()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName())
            .RuleFor(p => p.DateOfBirth, f => f.Date.Between(DateTime.Now.AddYears(-90), DateTime.Now))
            .RuleFor(p => p.Address, f => GenerateAddress())
            .RuleFor(p => p.MaritalStatus, (f, p) =>
            {
                var age = DateTime.Now.Year - p.DateOfBirth.Year;
                return age < 18 ? "Single" : f.PickRandom(new[] { "Single", "Married", "Divorced", "Widowed" });
            })
            .RuleFor(p => p.NumberOfDependents, (f, p) =>
            {
                var age = DateTime.Now.Year - p.DateOfBirth.Year;
                if (age < 18) return 0;

                if (age < 25) return f.Random.Int(0, 1);
                if (age < 35) return f.Random.Int(0, 2);
                if (age < 45) return f.Random.Int(0, 3);
                if (age < 55) return f.Random.Int(0, 4);
                return f.Random.Int(3, 5);
            })
            .RuleFor(p => p.HouseholdSize, (f, p) =>
            {
                var age = DateTime.Now.Year - p.DateOfBirth.Year;
                return age < 18 ? f.Random.Int(2, 5) : f.Random.Int(1, 6);
            }); ;

        return personFaker.Generate();
    }

    /// <summary>
    /// Generates a fake Address object.
    /// </summary>
    /// <returns>The generated Address object.</returns>
    public static Address GenerateAddress()
    {
        var addressFaker = new Faker<Address>()
            .RuleFor(a => a.StreetAddress, f => f.Address.StreetAddress())
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.State, f => f.Address.State())
            .RuleFor(a => a.PostalCode, f => f.Address.ZipCode())
            .RuleFor(a => a.Country, f => f.Address.Country());

        return addressFaker.Generate();
    }
}
