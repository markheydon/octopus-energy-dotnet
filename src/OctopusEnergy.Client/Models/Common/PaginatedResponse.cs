using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Common;

internal sealed class PaginatedResponse<TItem>
{
    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("next")]
    public string? Next { get; init; }

    [JsonPropertyName("previous")]
    public string? Previous { get; init; }

    [JsonPropertyName("results")]
    public IReadOnlyList<TItem> Results { get; init; } = [];
}
