namespace BatchProcessing.Domain.Models;

/// <summary>
/// Represents an address with street address, city, state, postal code, and country.
/// </summary>
public class Address
{
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public required string StreetAddress { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public required string City { get; set; }

    /// <summary>
    /// Gets or sets the state.
    /// </summary>
    public required string State { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public required string PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the country.
    /// </summary>
    public required string Country { get; set; }
}
