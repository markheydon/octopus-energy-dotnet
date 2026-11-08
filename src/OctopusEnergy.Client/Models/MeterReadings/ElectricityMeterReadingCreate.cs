using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// Input for submitting an electricity meter reading via GraphQL.
/// </summary>
public sealed class ElectricityMeterReadingCreate
{
    /// <summary>
    /// Gets the import MPAN for the meter point.
    /// </summary>
    [JsonPropertyName("mpan")]
    public required string Mpan { get; init; }

    /// <summary>
    /// Gets the electricity meter serial number.
    /// </summary>
    [JsonPropertyName("serialNumber")]
    public required string SerialNumber { get; init; }

    /// <summary>
    /// Gets the date of the reading (UK civil date derived from this instant).
    /// </summary>
    [JsonPropertyName("readAt")]
    public required DateTimeOffset ReadAt { get; init; }

    /// <summary>
    /// Gets one or more register readings for the meter.
    /// </summary>
    [JsonPropertyName("readings")]
    public required IReadOnlyList<ElectricityRegisterReading> Readings { get; init; }
}
