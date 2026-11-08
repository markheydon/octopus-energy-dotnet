using OctopusEnergy.Client.Models.RateLimits;

namespace OctopusEnergy.Client.Services.RateLimits;

/// <summary>
/// Kraken operational rate-limit information for the authenticated viewer.
/// </summary>
public interface IRateLimitService
{
    /// <summary>
    /// Fetches combined rate-limit information (hourly points allowance and related limits).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Rate limit information.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or is missing expected data.
    /// </exception>
    Task<RateLimitInfo> GetRateLimitInfoAsync(CancellationToken cancellationToken = default);
}
