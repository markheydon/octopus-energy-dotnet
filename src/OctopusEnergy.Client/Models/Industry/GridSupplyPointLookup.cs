using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Industry;

/// <summary>
/// Grid supply point resolved from a postcode industry lookup.
/// </summary>
public sealed class GridSupplyPointLookup
{
    /// <summary>
    /// Example MPAN for the postcode area.
    /// </summary>
    [JsonPropertyName("mpan")]
    public string Mpan { get; init; } = string.Empty;

    /// <summary>
    /// Postcode queried.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string Postcode { get; init; } = string.Empty;

    /// <summary>
    /// GSP group id (for example <c>_C</c> for London).
    /// </summary>
    [JsonPropertyName("group_id")]
    public GridSupplyPoint GridSupplyPoint { get; init; }

    /// <summary>
    /// GSP identifier returned by the API (same encoding as <see cref="GridSupplyPoint"/>).
    /// </summary>
    /// <remarks>
    /// The wire field duplicates <see cref="GridSupplyPoint"/> (<c>group_id</c>). Prefer
    /// <see cref="GridSupplyPoint"/> for new code.
    /// </remarks>
    [JsonPropertyName("gsp")]
    [Obsolete("Use GridSupplyPoint instead. The gsp wire field duplicates group_id on industry lookup responses.")]
    public GridSupplyPoint Gsp { get; init; }
}
