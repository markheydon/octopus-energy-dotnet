using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// A billing statement or invoice linked to a customer account.
/// </summary>
public sealed class AccountBill
{
    /// <summary>
    /// Kraken identifier for the bill.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Bill category (for example <c>STATEMENT</c>).
    /// </summary>
    [JsonPropertyName("billType")]
    public string? BillType { get; init; }

    /// <summary>
    /// Date the bill was issued (civil date).
    /// </summary>
    [JsonPropertyName("issuedDate")]
    public DateOnly? IssuedDate { get; init; }

    /// <summary>
    /// Short-lived URL to download the bill PDF, when available.
    /// </summary>
    [JsonPropertyName("temporaryUrl")]
    public string? TemporaryUrl { get; init; }
}
