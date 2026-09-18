using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// A register on an electricity meter.
/// </summary>
public sealed class MeterRegister
{
    /// <summary>
    /// Register identifier (for example <c>1</c> or <c>01</c>).
    /// </summary>
    [JsonPropertyName("identifier")]
    public string Identifier { get; init; } = string.Empty;

    /// <summary>
    /// Rate type (for example <c>STANDARD</c>, <c>DAY</c>, or <c>NIGHT</c>).
    /// </summary>
    [JsonPropertyName("rate")]
    public string Rate { get; init; } = string.Empty;

    /// <summary>
    /// Whether this is the settlement register.
    /// </summary>
    [JsonPropertyName("is_settlement_register")]
    public bool IsSettlementRegister { get; init; }
}
