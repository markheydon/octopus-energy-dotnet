using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Industry;

/// <summary>
/// Industry metadata for an electricity meter point (MPAN).
/// </summary>
public sealed class ElectricityMeterPointLookup
{
    /// <summary>
    /// Grid supply point for the MPAN.
    /// </summary>
    [JsonPropertyName("gsp")]
    public GridSupplyPoint GridSupplyPoint { get; init; }

    /// <summary>
    /// Profile class (for example 1 or 2).
    /// </summary>
    [JsonPropertyName("profile_class")]
    public int ProfileClass { get; init; }
}
