using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Common;

namespace OctopusEnergy.Client.Services.AccountBilling;

/// <summary>
/// GraphQL account billing lists (bills, transactions, payments) for the authenticated dashboard API key.
/// </summary>
public interface IAccountBillingService
{
    /// <summary>
    /// Lists one page of bills for an account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="request">Paging and filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Relay page of bills.</returns>
    Task<GraphQlRelayPage<AccountBill>> ListBillsPageAsync(
        string accountNumber,
        AccountBillListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists one page of ledger transactions for an account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="request">Paging and optional date filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Relay page of transactions.</returns>
    Task<GraphQlRelayPage<AccountTransaction>> ListTransactionsPageAsync(
        string accountNumber,
        AccountTransactionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists one page of payments for an account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="request">Paging options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Relay page of payments.</returns>
    Task<GraphQlRelayPage<AccountPayment>> ListPaymentsPageAsync(
        string accountNumber,
        AccountPaymentListRequest? request = null,
        CancellationToken cancellationToken = default);
}
