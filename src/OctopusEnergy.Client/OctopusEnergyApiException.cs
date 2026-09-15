using System.Net;

namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when the Octopus API returns a documented error payload
/// (for example REST <c>detail</c> or future GraphQL <c>extensions.errorCode</c>).
/// </summary>
public sealed class OctopusEnergyApiException : OctopusEnergyHttpException
{
    /// <summary>
    /// Creates an exception from a documented API error payload.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="detail">The API error detail message.</param>
    public OctopusEnergyApiException(HttpStatusCode statusCode, string detail)
        : base(statusCode, detail)
    {
        Detail = detail;
    }

    /// <summary>
    /// Gets the API error detail message.
    /// </summary>
    public string Detail { get; }
}
