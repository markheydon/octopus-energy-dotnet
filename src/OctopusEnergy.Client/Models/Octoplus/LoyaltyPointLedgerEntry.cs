using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// One ledger line from <c>loyaltyPointLedgers</c>.
/// </summary>
public sealed class LoyaltyPointLedgerEntry
{
    /// <summary>
    /// Ledger entry identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Ledger type label from Kraken.
    /// </summary>
    [JsonPropertyName("ledgerType")]
    public string? LedgerType { get; init; }

    /// <summary>
    /// Movement value as returned by the API (string wire format).
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; init; }

    /// <summary>
    /// Running balance after this entry (string wire format).
    /// </summary>
    [JsonPropertyName("balanceCarriedForward")]
    public string? BalanceCarriedForward { get; init; }

    /// <summary>
    /// When the entry was posted.
    /// </summary>
    [JsonPropertyName("postedAt")]
    public DateTimeOffset? PostedAt { get; init; }

    /// <summary>
    /// Related account number when present on the entry.
    /// </summary>
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; init; }
}
