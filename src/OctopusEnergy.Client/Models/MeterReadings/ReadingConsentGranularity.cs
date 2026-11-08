using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// Smart-meter reading consent granularity for an electricity meter point.
/// </summary>
public sealed class ReadingConsentGranularity
{
    /// <summary>
    /// Gets the consent granularity value returned by Kraken for the MPAN.
    /// </summary>
    [JsonPropertyName("granularityValue")]
    public string? GranularityValue { get; init; }
}
