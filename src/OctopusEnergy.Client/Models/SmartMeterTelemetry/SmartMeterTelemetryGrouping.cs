namespace OctopusEnergy.Client.Models.SmartMeterTelemetry;

/// <summary>
/// Granularity for ranged <c>smartMeterTelemetry</c> queries.
/// </summary>
/// <remarks>
/// Maps to the GraphQL <c>TelemetryGrouping</c> enum. <c>start</c>, <c>end</c>, and <c>grouping</c>
/// must all be supplied together when requesting a time range.
/// </remarks>
public enum SmartMeterTelemetryGrouping
{
    /// <summary>
    /// Ten-second buckets.
    /// </summary>
    TenSeconds,

    /// <summary>
    /// One-minute buckets.
    /// </summary>
    OneMinute,

    /// <summary>
    /// Five-minute buckets.
    /// </summary>
    FiveMinutes,

    /// <summary>
    /// Half-hourly buckets.
    /// </summary>
    HalfHourly,

    /// <summary>
    /// Hourly buckets.
    /// </summary>
    Hourly,
}
