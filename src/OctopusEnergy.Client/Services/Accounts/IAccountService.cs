using OctopusEnergy.Client.Models.Accounts;

namespace OctopusEnergy.Client.Services.Accounts;

/// <summary>
/// Customer account operations.
/// </summary>
public interface IAccountService
{
    /// <summary>
    /// Retrieves account detail including properties, meters, and agreements.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Account detail.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="accountNumber"/> is null or whitespace.
    /// </exception>
    /// <exception cref="OctopusEnergyApiException">
    /// Thrown when the API returns an error response (for example HTTP 404 for an unknown account).
    /// </exception>
    /// <exception cref="OctopusEnergyHttpException">
    /// Thrown when the API returns a non-success HTTP status without a parseable error body.
    /// </exception>
    /// <remarks>
    /// Requires authentication with a dashboard API key. When you do not have an account number,
    /// use <see cref="Viewer.IViewerService.GetViewerAsync(System.Threading.CancellationToken)"/> on
    /// <see cref="OctopusEnergyClient.Viewer"/> first; then call this method with the chosen number.
    /// </remarks>
    Task<Account> GetAsync(string accountNumber, CancellationToken cancellationToken = default);
}
