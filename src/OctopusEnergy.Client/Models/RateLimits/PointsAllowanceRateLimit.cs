using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.RateLimits;

/// <summary>
/// Hourly GraphQL complexity points allowance for the authenticated viewer.
/// </summary>
public sealed class PointsAllowanceRateLimit
{
    /// <summary>
    /// Gets the hourly points limit (default 50,000 for dashboard account users).
    /// </summary>
    [JsonPropertyName("limit")]
    public int? Limit { get; init; }

    /// <summary>
    /// Gets the remaining points in the current hour.
    /// </summary>
    [JsonPropertyName("remaining")]
    public int? Remaining { get; init; }
}
