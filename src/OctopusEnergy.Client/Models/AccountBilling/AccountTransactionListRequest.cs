using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// Paging and filters for listing account transactions via GraphQL.
/// </summary>
public sealed class AccountTransactionListRequest
{
    /// <summary>
    /// Gets the Relay page size (<c>first</c>). Defaults to <see cref="AccountBillingListDefaults.DefaultFirst"/>.
    /// </summary>
    [JsonPropertyName("first")]
    public int First { get; init; } = AccountBillingListDefaults.DefaultFirst;

    /// <summary>
    /// Gets the Relay <c>after</c> cursor for forward paging.
    /// </summary>
    [JsonPropertyName("after")]
    public string? After { get; init; }

    /// <summary>
    /// Gets an optional inclusive start date filter (<c>fromDate</c>).
    /// </summary>
    [JsonPropertyName("fromDate")]
    public DateOnly? FromDate { get; init; }

    /// <summary>
    /// Gets an optional inclusive end date filter (<c>toDate</c>).
    /// </summary>
    [JsonPropertyName("toDate")]
    public DateOnly? ToDate { get; init; }
}
