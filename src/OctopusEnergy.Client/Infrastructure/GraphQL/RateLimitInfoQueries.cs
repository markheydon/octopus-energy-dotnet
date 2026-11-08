namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class RateLimitInfoQueries
{
    internal const string GetRateLimitInfo = """
        query RateLimitInfo {
          rateLimitInfo {
            pointsAllowanceRateLimit {
              limit
              remaining
            }
          }
        }
        """;
}
