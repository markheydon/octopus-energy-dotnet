using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Octoplus;

/// <summary>
/// Outcome of <c>joinOctoplusCampaign</c>.
/// </summary>
public sealed class JoinOctoplusCampaignResult
{
    /// <summary>
    /// Whether Octoplus enrolment succeeded according to the mutation response.
    /// </summary>
    [JsonPropertyName("hasEnrolled")]
    public bool? HasEnrolled { get; init; }
}
