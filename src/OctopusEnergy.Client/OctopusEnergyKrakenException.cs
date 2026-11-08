using System.Net;

namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when the Kraken GraphQL API returns <c>errors.extensions.errorCode</c>.
/// </summary>
public class OctopusEnergyKrakenException : OctopusEnergyApiException
{
    /// <summary>
    /// Creates an exception from a Kraken GraphQL error.
    /// </summary>
    /// <param name="krakenErrorCode">The <c>extensions.errorCode</c> value.</param>
    /// <param name="message">The API error message.</param>
    public OctopusEnergyKrakenException(string krakenErrorCode, string? message)
        : base(HttpStatusCode.OK, FormatDetail(krakenErrorCode, message))
    {
        KrakenErrorCode = krakenErrorCode;
    }

    /// <summary>
    /// Gets the Kraken <c>extensions.errorCode</c> value.
    /// </summary>
    public string KrakenErrorCode { get; }

    internal static string FormatDetail(string krakenErrorCode, string? message)
    {
        string text = message ?? "The GraphQL API returned an error.";
        return string.IsNullOrEmpty(krakenErrorCode) ? text : $"{krakenErrorCode}: {text}";
    }
}
