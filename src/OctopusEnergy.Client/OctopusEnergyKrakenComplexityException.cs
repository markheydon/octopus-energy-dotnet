namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when a GraphQL request exceeds the documented per-request complexity limit (<c>KT-CT-1188</c>).
/// </summary>
public sealed class OctopusEnergyKrakenComplexityException : OctopusEnergyKrakenException
{
    /// <summary>
    /// Creates an exception from a Kraken complexity-limit error.
    /// </summary>
    /// <param name="krakenErrorCode">The error code (expected <c>KT-CT-1188</c>).</param>
    /// <param name="message">The API error message.</param>
    public OctopusEnergyKrakenComplexityException(string krakenErrorCode, string? message)
        : base(krakenErrorCode, message)
    {
    }
}
