using OctopusEnergy.Client.Models.Octoplus;

namespace OctopusEnergy.Client.Services.Octoplus;

/// <summary>
/// Optional GraphQL Octoplus helpers: enrolment, loyalty points, and wheel-of-fortune allowance.
/// </summary>
/// <remarks>
/// Operations target the public Kraken GraphQL host only.
/// Saving-session join flows live on <c>api.backend.octopus.energy</c> and are out of SDK scope per <c>SCOPE.md</c>.
/// </remarks>
public interface IOctoplusService
{
    /// <summary>
    /// Fetches Octoplus enrolment metadata for an account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<OctoplusAccountInfo> GetAccountInfoAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists loyalty point ledger lines visible to the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ledger lines. The list is empty when none are returned; it is never null.</returns>
    Task<IReadOnlyList<LoyaltyPointLedgerEntry>> ListLoyaltyPointLedgersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the loyalty points balance for an account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="schemeCode">Optional loyalty scheme code when the account has multiple schemes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<LoyaltyPointsBalance> GetLoyaltyPointsBalanceAsync(
        string accountNumber,
        string? schemeCode = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches monthly wheel-of-fortune spin allowance (deprecated GraphQL field, still customer-reachable).
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<WheelOfFortuneSpinsAvailability> GetWheelOfFortuneSpinsAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to enrol the account in the Octoplus campaign.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<JoinOctoplusCampaignResult> JoinCampaignAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Redeems loyalty points into account credit.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="points">Points to redeem (must be positive).</param>
    /// <param name="schemeCode">Optional loyalty scheme code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<RedeemLoyaltyPointsResult> RedeemPointsForAccountCreditAsync(
        string accountNumber,
        int points,
        string? schemeCode = null,
        CancellationToken cancellationToken = default);
}
