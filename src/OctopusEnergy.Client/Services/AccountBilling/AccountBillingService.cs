using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Common;

namespace OctopusEnergy.Client.Services.AccountBilling;

/// <summary>
/// GraphQL account billing lists for the authenticated dashboard API key.
/// </summary>
public sealed class AccountBillingService : IAccountBillingService
{
    private readonly GraphQlClient? _graphQl;

    internal AccountBillingService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<GraphQlRelayPage<AccountBill>> ListBillsPageAsync(
        string accountNumber,
        AccountBillListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        request ??= new AccountBillListRequest();
        ValidateAccountNumber(accountNumber);
        GraphQlRelayLimits.ValidateFirst(request.First);

        AccountBillsQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
            First = request.First,
            After = string.IsNullOrWhiteSpace(request.After) ? null : request.After.Trim(),
            IncludeBillsWithoutPdf = request.IncludeBillsWithoutPdf,
        };

        bool pagingWithAfter = variables.After is not null;

        AccountBillsQueryData data = await _graphQl!.ExecuteAsync<AccountBillsQueryData>(
            AccountBillingQueries.ListBills,
            variables,
            cancellationToken).ConfigureAwait(false);

        AccountBillsAccountWire account = data.Account
            ?? throw new OctopusEnergyParseException(
                "The account billing query returned no account data for the supplied account number.");

        return AccountBillingRelayMapper.MapBills(
            account.Bills,
            pagingWithAfter);
    }

    /// <inheritdoc />
    public async Task<GraphQlRelayPage<AccountTransaction>> ListTransactionsPageAsync(
        string accountNumber,
        AccountTransactionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        request ??= new AccountTransactionListRequest();
        ValidateAccountNumber(accountNumber);
        GraphQlRelayLimits.ValidateFirst(request.First);

        AccountTransactionsQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
            First = request.First,
            After = string.IsNullOrWhiteSpace(request.After) ? null : request.After.Trim(),
            FromDate = FormatDate(request.FromDate),
            ToDate = FormatDate(request.ToDate),
        };

        bool pagingWithAfter = variables.After is not null;

        AccountTransactionsQueryData data = await _graphQl!.ExecuteAsync<AccountTransactionsQueryData>(
            AccountBillingQueries.ListTransactions,
            variables,
            cancellationToken).ConfigureAwait(false);

        AccountTransactionsAccountWire account = data.Account
            ?? throw new OctopusEnergyParseException(
                "The account billing query returned no account data for the supplied account number.");

        return AccountBillingRelayMapper.MapTransactions(
            account.Transactions,
            pagingWithAfter);
    }

    /// <inheritdoc />
    public async Task<GraphQlRelayPage<AccountPayment>> ListPaymentsPageAsync(
        string accountNumber,
        AccountPaymentListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        request ??= new AccountPaymentListRequest();
        ValidateAccountNumber(accountNumber);
        GraphQlRelayLimits.ValidateFirst(request.First);

        AccountPaymentsQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
            First = request.First,
            After = string.IsNullOrWhiteSpace(request.After) ? null : request.After.Trim(),
        };

        bool pagingWithAfter = variables.After is not null;

        AccountPaymentsQueryData data = await _graphQl!.ExecuteAsync<AccountPaymentsQueryData>(
            AccountBillingQueries.ListPayments,
            variables,
            cancellationToken).ConfigureAwait(false);

        AccountPaymentsAccountWire account = data.Account
            ?? throw new OctopusEnergyParseException(
                "The account billing query returned no account data for the supplied account number.");

        return AccountBillingRelayMapper.MapPayments(
            account.Payments,
            pagingWithAfter);
    }

    private void EnsureGraphQl()
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to list account billing via GraphQL.");
        }
    }

    private static void ValidateAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new OctopusEnergyRequestException("Account number is required.");
        }
    }

    private static string? FormatDate(DateOnly? date)
    {
        return date?.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
    }
}
