using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.RateLimits;

/// <summary>
/// Combined Kraken rate-limit information for the authenticated viewer.
/// </summary>
public sealed class RateLimitInfo
{
    /// <summary>
    /// Gets hourly complexity points allowance and remaining balance.
    /// </summary>
    [JsonPropertyName("pointsAllowanceRateLimit")]
    public PointsAllowanceRateLimit? PointsAllowanceRateLimit { get; init; }
}
