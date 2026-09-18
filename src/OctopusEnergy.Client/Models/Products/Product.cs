using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Energy product summary from the catalogue list endpoint.
/// </summary>
public class Product
{
    /// <summary>
    /// Product code (for example <c>AGILE-FLEX-22-11-25</c>).
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Import or export direction.
    /// </summary>
    [JsonPropertyName("direction")]
    public string? Direction { get; init; }

    /// <summary>
    /// Full product name.
    /// </summary>
    [JsonPropertyName("full_name")]
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Display name shown to customers.
    /// </summary>
    [JsonPropertyName("display_name")]
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Marketing description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Whether the product has a variable tariff.
    /// </summary>
    [JsonPropertyName("is_variable")]
    public bool IsVariable { get; init; }

    /// <summary>
    /// Whether the product is marketed as green.
    /// </summary>
    [JsonPropertyName("is_green")]
    public bool IsGreen { get; init; }

    /// <summary>
    /// Whether the product is a tracker tariff.
    /// </summary>
    [JsonPropertyName("is_tracker")]
    public bool IsTracker { get; init; }

    /// <summary>
    /// Whether the product is for prepayment meters.
    /// </summary>
    [JsonPropertyName("is_prepay")]
    public bool IsPrepay { get; init; }

    /// <summary>
    /// Whether the product is for business customers.
    /// </summary>
    [JsonPropertyName("is_business")]
    public bool IsBusiness { get; init; }

    /// <summary>
    /// Whether the product is restricted.
    /// </summary>
    [JsonPropertyName("is_restricted")]
    public bool IsRestricted { get; init; }

    /// <summary>
    /// Fixed-term length in months, or <see langword="null"/> for variable products.
    /// </summary>
    [JsonPropertyName("term")]
    public int? Term { get; init; }

    /// <summary>
    /// When the product became available.
    /// </summary>
    [JsonPropertyName("available_from")]
    public DateTimeOffset? AvailableFrom { get; init; }

    /// <summary>
    /// When the product stopped being available, or <see langword="null"/> if still available.
    /// </summary>
    [JsonPropertyName("available_to")]
    public DateTimeOffset? AvailableTo { get; init; }

    /// <summary>
    /// Retail brand (for example <c>OCTOPUS_ENERGY</c>). Other brands may appear on the UK host.
    /// </summary>
    [JsonPropertyName("brand")]
    public string Brand { get; init; } = string.Empty;

    /// <summary>
    /// Hypermedia links for the product.
    /// </summary>
    [JsonPropertyName("links")]
    public IReadOnlyList<ProductLink> Links { get; init; } = [];
}
