using OctopusEnergy.Client.Models.Viewer;

namespace OctopusEnergy.Client.Services.Viewer;

/// <summary>
/// GraphQL viewer discovery for the authenticated dashboard API key.
/// </summary>
public interface IViewerService
{
    /// <summary>
    /// Fetches the viewer and discoverable account numbers for the authenticated API key.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Viewer detail. <see cref="ViewerDetail.AccountNumbers"/> is empty when the key has no linked accounts;
    /// it is never null.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or is missing expected data.
    /// </exception>
    Task<ViewerDetail> GetViewerAsync(CancellationToken cancellationToken = default);
}
