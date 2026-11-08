using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// Monthly wheel-of-fortune spin allowance for electricity and gas.
/// </summary>
public sealed class WheelOfFortuneSpinsAvailability
{
    /// <summary>
    /// Spin counts for electricity.
    /// </summary>
    [JsonPropertyName("electricity")]
    public FuelSpinsAvailability? Electricity { get; init; }

    /// <summary>
    /// Spin counts for gas.
    /// </summary>
    [JsonPropertyName("gas")]
    public FuelSpinsAvailability? Gas { get; init; }
}

/// <summary>
/// Wheel-of-fortune spin counts for one fuel type.
/// </summary>
public sealed class FuelSpinsAvailability
{
    /// <summary>
    /// Maximum spins allowed per month.
    /// </summary>
    [JsonPropertyName("maxSpinsPerMonth")]
    public int? MaxSpinsPerMonth { get; init; }

    /// <summary>
    /// Spins remaining in the current month.
    /// </summary>
    [JsonPropertyName("remainingSpinsThisMonth")]
    public int? RemainingSpinsThisMonth { get; init; }

    /// <summary>
    /// Spins already used this month.
    /// </summary>
    [JsonPropertyName("usedSpinsThisMonth")]
    public int? UsedSpinsThisMonth { get; init; }
}
