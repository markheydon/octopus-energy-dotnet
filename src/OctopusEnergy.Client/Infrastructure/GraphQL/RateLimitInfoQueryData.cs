using System.Text.Json.Serialization;
using OctopusEnergy.Client.Models.RateLimits;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class RateLimitInfoQueryData
{
    [JsonPropertyName("rateLimitInfo")]
    public RateLimitInfo? RateLimitInfo { get; init; }
}
