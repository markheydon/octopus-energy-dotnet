namespace OctopusEnergy.Client.Models.Consumption;

/// <summary>
/// REST <c>order_by</c> values for consumption lists.
/// </summary>
public enum ConsumptionOrderBy
{
    /// <summary>Newest intervals first (<c>-period</c>).</summary>
    PeriodDescending,

    /// <summary>Oldest intervals first (<c>period</c>).</summary>
    PeriodAscending,
}
