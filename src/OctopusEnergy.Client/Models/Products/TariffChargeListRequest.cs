namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Query parameters for tariff standing-charge and unit-rate list endpoints.
/// </summary>
public sealed class TariffChargeListRequest
{
    /// <summary>
    /// Start of the period to return (ISO 8601 with <c>Z</c> or offset).
    /// </summary>
    public DateTimeOffset? PeriodFrom { get; init; }

    /// <summary>
    /// End of the period to return (ISO 8601 with <c>Z</c> or offset).
    /// </summary>
    public DateTimeOffset? PeriodTo { get; init; }

    /// <summary>
    /// Page size for the first request. Default 100; maximum 1,500.
    /// </summary>
    public int? PageSize { get; init; }
}
