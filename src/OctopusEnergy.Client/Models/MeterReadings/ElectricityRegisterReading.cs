using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// A register reading value for an electricity meter submission.
/// </summary>
public sealed class ElectricityRegisterReading
{
    /// <summary>
    /// Gets the optional register identifier for the meter (required on some multi-register meters).
    /// </summary>
    [JsonPropertyName("registerId")]
    public string? RegisterId { get; init; }

    /// <summary>
    /// Gets the dial or register reading value.
    /// </summary>
    [JsonPropertyName("reading")]
    public required int Reading { get; init; }
}
