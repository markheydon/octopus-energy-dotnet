using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// Net, tax, and gross amounts for a ledger transaction (values in pence).
/// </summary>
public sealed class TransactionAmounts
{
    /// <summary>
    /// Net amount in pence.
    /// </summary>
    [JsonPropertyName("net")]
    public int? Net { get; init; }

    /// <summary>
    /// Tax amount in pence.
    /// </summary>
    [JsonPropertyName("tax")]
    public int? Tax { get; init; }

    /// <summary>
    /// Gross amount in pence.
    /// </summary>
    [JsonPropertyName("gross")]
    public int? Gross { get; init; }
}
