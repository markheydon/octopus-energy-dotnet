namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when a GraphQL request exceeds the documented node limit (<c>KT-CT-1189</c>).
/// </summary>
public sealed class OctopusEnergyKrakenNodeLimitException : OctopusEnergyKrakenException
{
    /// <summary>
    /// Creates an exception from a Kraken node-limit error.
    /// </summary>
    /// <param name="krakenErrorCode">The error code (expected <c>KT-CT-1189</c>).</param>
    /// <param name="message">The API error message.</param>
    public OctopusEnergyKrakenNodeLimitException(string krakenErrorCode, string? message)
        : base(krakenErrorCode, message)
    {
    }
}
