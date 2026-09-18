using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Common;

internal sealed class ApiErrorResponse
{
    [JsonPropertyName("detail")]
    public string? Detail { get; init; }
}
