namespace OctopusEnergy.Client.Models.Consumption;

/// <summary>
/// Query parameters for electricity and gas consumption list endpoints.
/// </summary>
public sealed class ConsumptionListRequest
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
    /// Page size for the first request. Default 100; maximum 25,000.
    /// </summary>
    public int? PageSize { get; init; }

    /// <summary>
    /// Sort order. Default is newest first.
    /// </summary>
    public ConsumptionOrderBy? OrderBy { get; init; }

    /// <summary>
    /// Optional aggregation bucket.
    /// </summary>
    public ConsumptionGroupBy? GroupBy { get; init; }
}
