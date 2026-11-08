using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// Paging options for listing account payments via GraphQL.
/// </summary>
public sealed class AccountPaymentListRequest
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
}
