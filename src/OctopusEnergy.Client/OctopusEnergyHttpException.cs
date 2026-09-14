using System.Net;

namespace OctopusEnergy.Client;

/// <summary>
/// Thrown when the Octopus API returns a non-success HTTP status code
/// that cannot be mapped to a documented API error payload.
/// </summary>
public class OctopusEnergyHttpException : OctopusEnergyException
{
    /// <summary>
    /// Creates an exception for the specified HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="message">The error message.</param>
    public OctopusEnergyHttpException(HttpStatusCode statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Creates an exception for the specified HTTP status code with an inner exception.
    /// </summary>
    /// <param name="statusCode">The HTTP status code returned by the API.</param>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OctopusEnergyHttpException(HttpStatusCode statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }
}
