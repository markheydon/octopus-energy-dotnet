using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// A ledger transaction on a customer account.
/// </summary>
public sealed class AccountTransaction
{
    /// <summary>
    /// Kraken identifier for the transaction.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Date the transaction was posted (civil date).
    /// </summary>
    [JsonPropertyName("postedDate")]
    public DateOnly? PostedDate { get; init; }

    /// <summary>
    /// GraphQL concrete type name (for example <c>Payment</c>, <c>Charge</c>).
    /// </summary>
    [JsonPropertyName("krakenType")]
    public string? KrakenType { get; init; }

    /// <summary>
    /// Net, tax, and gross amounts in pence.
    /// </summary>
    [JsonPropertyName("amounts")]
    public TransactionAmounts? Amounts { get; init; }
}
