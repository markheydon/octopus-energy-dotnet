using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// Octoplus enrolment status for an account.
/// </summary>
public sealed class OctoplusAccountInfo
{
    /// <summary>
    /// Kraken <c>enrollmentStatus</c> value (for example <c>ENROLLED</c> or <c>UNENROLLED_NEVER_ENROLLED</c>).
    /// </summary>
    [JsonPropertyName("enrollmentStatus")]
    public string? EnrollmentStatus { get; init; }

    /// <summary>
    /// Whether the account is registered on the loyalty points scheme.
    /// </summary>
    [JsonPropertyName("isLoyaltyPointsUser")]
    public bool? IsLoyaltyPointsUser { get; init; }
}
