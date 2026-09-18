namespace OctopusEnergy.Client.Models.Consumption;

/// <summary>
/// REST <c>group_by</c> values for consumption aggregation.
/// </summary>
/// <remarks>
/// <c>day</c> uses local midnight in Europe/London, not UTC.
/// </remarks>
public enum ConsumptionGroupBy
{
    /// <summary>Half-hourly or hourly buckets.</summary>
    Hour,

    /// <summary>Daily buckets at local midnight.</summary>
    Day,

    /// <summary>Weekly buckets.</summary>
    Week,

    /// <summary>Monthly buckets.</summary>
    Month,

    /// <summary>Quarterly buckets.</summary>
    Quarter,
}
