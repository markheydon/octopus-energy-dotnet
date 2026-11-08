using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class OctoplusAccountInfoQueryData
{
    [JsonPropertyName("octoplusAccountInfo")]
    public OctoplusAccountInfoWire? OctoplusAccountInfo { get; init; }
}

internal sealed class OctoplusAccountInfoWire
{
    [JsonPropertyName("enrollmentStatus")]
    public string? EnrollmentStatus { get; init; }

    [JsonPropertyName("isLoyaltyPointsUser")]
    public bool? IsLoyaltyPointsUser { get; init; }
}

internal sealed class LoyaltyPointLedgersQueryData
{
    [JsonPropertyName("loyaltyPointLedgers")]
    public IReadOnlyList<LoyaltyPointLedgerEntryWire>? LoyaltyPointLedgers { get; init; }
}

internal sealed class LoyaltyPointLedgerEntryWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("ledgerType")]
    public string? LedgerType { get; init; }

    [JsonPropertyName("value")]
    public string? Value { get; init; }

    [JsonPropertyName("balanceCarriedForward")]
    public string? BalanceCarriedForward { get; init; }

    [JsonPropertyName("postedAt")]
    public DateTimeOffset? PostedAt { get; init; }

    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; init; }
}

internal sealed class LoyaltyPointsBalanceQueryData
{
    [JsonPropertyName("loyaltyPointsBalance")]
    public LoyaltyPointsBalanceWire? LoyaltyPointsBalance { get; init; }
}

internal sealed class LoyaltyPointsBalanceWire
{
    [JsonPropertyName("loyaltyPoints")]
    public int? LoyaltyPoints { get; init; }

    [JsonPropertyName("totalMonetaryAmount")]
    public int? TotalMonetaryAmount { get; init; }
}

internal sealed class LoyaltyPointsBalanceQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("schemeCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SchemeCode { get; init; }
}

internal sealed class WheelOfFortuneSpinsQueryData
{
    [JsonPropertyName("wheelOfFortuneSpins")]
    public WheelOfFortuneSpinsWire? WheelOfFortuneSpins { get; init; }
}

internal sealed class WheelOfFortuneSpinsWire
{
    [JsonPropertyName("electricity")]
    public FuelSpinsWire? Electricity { get; init; }

    [JsonPropertyName("gas")]
    public FuelSpinsWire? Gas { get; init; }
}

internal sealed class FuelSpinsWire
{
    [JsonPropertyName("maxSpinsPerMonth")]
    public int? MaxSpinsPerMonth { get; init; }

    [JsonPropertyName("remainingSpinsThisMonth")]
    public int? RemainingSpinsThisMonth { get; init; }

    [JsonPropertyName("usedSpinsThisMonth")]
    public int? UsedSpinsThisMonth { get; init; }
}

internal sealed class OctoplusAccountInfoQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }
}

internal sealed class WheelOfFortuneSpinsQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }
}

internal sealed class JoinOctoplusCampaignMutationData
{
    [JsonPropertyName("joinOctoplusCampaign")]
    public JoinOctoplusCampaignWire? JoinOctoplusCampaign { get; init; }
}

internal sealed class JoinOctoplusCampaignWire
{
    [JsonPropertyName("outcome")]
    public JoinOctoplusCampaignOutcomeWire? Outcome { get; init; }
}

internal sealed class JoinOctoplusCampaignOutcomeWire
{
    [JsonPropertyName("octoplus")]
    public OctoplusEnrolmentOutcomeWire? Octoplus { get; init; }
}

internal sealed class OctoplusEnrolmentOutcomeWire
{
    [JsonPropertyName("hasEnrolled")]
    public bool? HasEnrolled { get; init; }
}

internal sealed class JoinOctoplusCampaignMutationVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }
}

internal sealed class RedeemLoyaltyPointsMutationData
{
    [JsonPropertyName("redeemLoyaltyPointsForAccountCredit")]
    public RedeemLoyaltyPointsWire? RedeemLoyaltyPointsForAccountCredit { get; init; }
}

internal sealed class RedeemLoyaltyPointsWire
{
    [JsonPropertyName("pointsRedeemed")]
    public int? PointsRedeemed { get; init; }
}

internal sealed class RedeemLoyaltyPointsMutationVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("points")]
    public required int Points { get; init; }

    [JsonPropertyName("schemeCode")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SchemeCode { get; init; }
}
