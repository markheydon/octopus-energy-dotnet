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
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the account number is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised, the account is missing from the payload, or a connection is absent.
    /// </exception>
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
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the account number is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised, the account is missing from the payload, or a connection is absent.
    /// </exception>
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
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the account number is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised, the account is missing from the payload, or a connection is absent.
    /// </exception>
    Task<GraphQlRelayPage<AccountPayment>> ListPaymentsPageAsync(
        string accountNumber,
        AccountPaymentListRequest? request = null,
        CancellationToken cancellationToken = default);
}
