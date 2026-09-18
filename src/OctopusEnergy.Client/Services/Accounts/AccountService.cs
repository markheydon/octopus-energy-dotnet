using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Models.Accounts;

namespace OctopusEnergy.Client.Services.Accounts;

/// <summary>
/// Customer account operations.
/// </summary>
public sealed class AccountService
{
    private const string AccountsPathPrefix = "accounts/";

    private readonly RestClient _rest;

    internal AccountService(RestClient rest)
    {
        ArgumentNullException.ThrowIfNull(rest);
        _rest = rest;
    }

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
    /// Requires authentication with a dashboard API key. Discovering the account number without
    /// a bill is a v2 GraphQL <c>viewer</c> story; callers supply the account number for v1 REST.
    /// </remarks>
    public Task<Account> GetAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new OctopusEnergyRequestException("Account number is required.");
        }

        string path = $"{AccountsPathPrefix}{Uri.EscapeDataString(accountNumber)}/";
        return _rest.GetAsync<Account>(path, cancellationToken);
    }
}
