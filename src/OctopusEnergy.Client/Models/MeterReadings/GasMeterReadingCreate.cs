using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// Input for submitting a gas meter reading via GraphQL.
/// </summary>
public sealed class GasMeterReadingCreate
{
    /// <summary>
    /// Gets the MPRN for the meter point.
    /// </summary>
    [JsonPropertyName("mprn")]
    public required string Mprn { get; init; }

    /// <summary>
    /// Gets the gas meter serial number.
    /// </summary>
    [JsonPropertyName("serialNumber")]
    public required string SerialNumber { get; init; }

    /// <summary>
    /// Gets the date of the reading (UK civil date derived from this instant).
    /// </summary>
    [JsonPropertyName("readAt")]
    public required DateTimeOffset ReadAt { get; init; }

    /// <summary>
    /// Gets the meter register reading value.
    /// </summary>
    [JsonPropertyName("reading")]
    public required int Reading { get; init; }
}
