using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// Current Octoplus / loyalty points balance for an account.
/// </summary>
public sealed class LoyaltyPointsBalance
{
    /// <summary>
    /// Points available to spend or redeem.
    /// </summary>
    [JsonPropertyName("loyaltyPoints")]
    public int? Points { get; init; }

    /// <summary>
    /// Monetary equivalent in pence when provided by the API.
    /// </summary>
    [JsonPropertyName("totalMonetaryAmount")]
    public int? TotalMonetaryAmountPence { get; init; }
}
