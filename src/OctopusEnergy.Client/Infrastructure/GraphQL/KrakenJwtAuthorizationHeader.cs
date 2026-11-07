namespace OctopusEnergy.Client.Infrastructure.GraphQL;

/// <summary>
/// Formats the GraphQL <c>Authorization</c> header after a live Kraken token is obtained.
/// </summary>
internal static class KrakenJwtAuthorizationHeader
{
    /// <summary>
    /// Returns the full wire value for the <c>Authorization</c> header (raw Kraken token, no <c>JWT</c> or <c>Token</c> prefix).
    /// </summary>
    /// <param name="krakenToken">The token returned by <c>obtainKrakenToken</c>.</param>
    internal static string Format(string krakenToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(krakenToken);

        return krakenToken;
    }
}
