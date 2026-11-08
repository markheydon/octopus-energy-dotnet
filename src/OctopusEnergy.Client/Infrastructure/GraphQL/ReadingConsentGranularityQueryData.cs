using System.Text.Json.Serialization;
using OctopusEnergy.Client.Models.MeterReadings;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class ReadingConsentGranularityQueryData
{
    [JsonPropertyName("readingConsentGranularity")]
    public ReadingConsentGranularity? ReadingConsentGranularity { get; init; }
}

internal sealed class ReadingConsentGranularityQueryVariables
{
    [JsonPropertyName("mpan")]
    public required string Mpan { get; init; }
}
