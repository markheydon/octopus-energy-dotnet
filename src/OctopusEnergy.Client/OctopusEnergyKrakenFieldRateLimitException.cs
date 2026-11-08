namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when a GraphQL field-specific rate limit is exceeded (<c>KT-CT-1199</c>).
/// </summary>
public sealed class OctopusEnergyKrakenFieldRateLimitException : OctopusEnergyKrakenException
{
    /// <summary>
    /// Creates an exception from a Kraken field rate-limit error.
    /// </summary>
    /// <param name="krakenErrorCode">The error code (expected <c>KT-CT-1199</c>).</param>
    /// <param name="message">The API error message.</param>
    public OctopusEnergyKrakenFieldRateLimitException(string krakenErrorCode, string? message)
        : base(krakenErrorCode, message)
    {
    }
}
