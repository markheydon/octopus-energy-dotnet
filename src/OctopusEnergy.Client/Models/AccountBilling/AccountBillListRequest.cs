using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.AccountBilling;

/// <summary>
/// Paging and filters for listing account bills via GraphQL.
/// </summary>
public sealed class AccountBillListRequest
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
    /// Gets whether to include bills that do not yet have a PDF.
    /// </summary>
    [JsonPropertyName("includeBillsWithoutPdf")]
    public bool IncludeBillsWithoutPdf { get; init; } = true;
}
