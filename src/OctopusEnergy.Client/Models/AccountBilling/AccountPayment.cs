using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// A payment on a customer account ledger.
/// </summary>
public sealed class AccountPayment
{
    /// <summary>
    /// Kraken identifier for the payment.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Payment amount in pence.
    /// </summary>
    [JsonPropertyName("amount")]
    public int? Amount { get; init; }

    /// <summary>
    /// Date the payment was or will be made (civil date).
    /// </summary>
    [JsonPropertyName("paymentDate")]
    public DateOnly? PaymentDate { get; init; }
}
