using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// Outcome of <c>redeemLoyaltyPointsForAccountCredit</c>.
/// </summary>
public sealed class RedeemLoyaltyPointsResult
{
    /// <summary>
    /// Points redeemed into account credit.
    /// </summary>
    [JsonPropertyName("pointsRedeemed")]
    public int? PointsRedeemed { get; init; }
}
