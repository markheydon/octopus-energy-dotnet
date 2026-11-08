namespace OctopusEnergy.Client.Models.SmartMeterTelemetry;

/// <summary>
/// Time range and grouping for <c>smartMeterTelemetry</c>.
/// </summary>
/// <remarks>
/// All three values are required together; partial ranges are rejected by the API (<c>KT-GB-4040</c>).
/// </remarks>
public sealed class SmartMeterTelemetryRange
{
    /// <summary>
    /// Gets the inclusive range start.
    /// </summary>
    public required DateTimeOffset Start { get; init; }

    /// <summary>
    /// Gets the exclusive range end.
    /// </summary>
    public required DateTimeOffset End { get; init; }

    /// <summary>
    /// Gets the aggregation granularity.
    /// </summary>
    public required SmartMeterTelemetryGrouping Grouping { get; init; }
}
