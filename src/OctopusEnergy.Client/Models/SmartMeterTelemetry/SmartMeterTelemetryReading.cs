namespace OctopusEnergy.Client.Models.SmartMeterTelemetry;

/// <summary>
/// A single telemetry sample from <c>smartMeterTelemetry</c>.
/// </summary>
public sealed class SmartMeterTelemetryReading
{
    /// <summary>
    /// Gets the sample timestamp when returned by the API.
    /// </summary>
    public DateTimeOffset? ReadAt { get; init; }

    /// <summary>
    /// Gets cumulative consumption when returned by the API.
    /// </summary>
    public decimal? Consumption { get; init; }

    /// <summary>
    /// Gets consumption change for the grouping period when returned by the API.
    /// </summary>
    public decimal? ConsumptionDelta { get; init; }

    /// <summary>
    /// Gets cost change for the grouping period when returned by the API.
    /// </summary>
    public decimal? CostDelta { get; init; }

    /// <summary>
    /// Gets cost change including tax when returned by the API.
    /// </summary>
    public decimal? CostDeltaWithTax { get; init; }

    /// <summary>
    /// Gets demand when returned by the API.
    /// </summary>
    public decimal? Demand { get; init; }

    /// <summary>
    /// Gets export energy when returned by the API.
    /// </summary>
    public decimal? Export { get; init; }
}
