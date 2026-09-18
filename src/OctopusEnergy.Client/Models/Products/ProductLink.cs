using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Hypermedia link on a product or tariff resource.
/// </summary>
public sealed class ProductLink
{
    /// <summary>
    /// Absolute URL for the linked resource.
    /// </summary>
    [JsonPropertyName("href")]
    public string Href { get; init; } = string.Empty;

    /// <summary>
    /// HTTP method for the link (typically <c>GET</c>).
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; init; } = string.Empty;

    /// <summary>
    /// Link relation (for example <c>self</c>, <c>standing_charges</c>).
    /// </summary>
    [JsonPropertyName("rel")]
    public string Rel { get; init; } = string.Empty;
}
