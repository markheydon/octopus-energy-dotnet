using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// A gas meter at a meter point.
/// </summary>
public sealed class GasMeter
{
    /// <summary>
    /// Meter serial number.
    /// </summary>
    [JsonPropertyName("serial_number")]
    public string SerialNumber { get; init; } = string.Empty;
}
