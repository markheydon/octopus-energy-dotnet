using System.Globalization;
using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Common;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class AccountBillingRelayMapper
{
    internal static GraphQlRelayPage<AccountBill> MapBills(
        RelayConnectionWire<AccountBillWire>? connection,
        bool pagingWithAfter)
    {
        if (connection is null)
        {
            return new GraphQlRelayPage<AccountBill>();
        }

        List<AccountBill> results = [];

        if (connection.Edges is not null)
        {
            foreach (RelayEdgeWire<AccountBillWire> edge in connection.Edges)
            {
                if (edge.Node is null)
                {
                    continue;
                }

                results.Add(MapBill(edge.Node));
            }
        }

        return MapPageInfo(connection.PageInfo, results, pagingWithAfter);
    }

    internal static GraphQlRelayPage<AccountTransaction> MapTransactions(
        RelayConnectionWire<AccountTransactionWire>? connection,
        bool pagingWithAfter)
    {
        if (connection is null)
        {
            return new GraphQlRelayPage<AccountTransaction>();
        }

        List<AccountTransaction> results = [];

        if (connection.Edges is not null)
        {
            foreach (RelayEdgeWire<AccountTransactionWire> edge in connection.Edges)
            {
                if (edge.Node is null)
                {
                    continue;
                }

                results.Add(MapTransaction(edge.Node));
            }
        }

        return MapPageInfo(connection.PageInfo, results, pagingWithAfter);
    }

    internal static GraphQlRelayPage<AccountPayment> MapPayments(
        RelayConnectionWire<AccountPaymentWire>? connection,
        bool pagingWithAfter)
    {
        if (connection is null)
        {
            return new GraphQlRelayPage<AccountPayment>();
        }

        List<AccountPayment> results = [];

        if (connection.Edges is not null)
        {
            foreach (RelayEdgeWire<AccountPaymentWire> edge in connection.Edges)
            {
                if (edge.Node is null)
                {
                    continue;
                }

                results.Add(MapPayment(edge.Node));
            }
        }

        return MapPageInfo(connection.PageInfo, results, pagingWithAfter);
    }

    private static GraphQlRelayPage<TItem> MapPageInfo<TItem>(
        RelayPageInfoWire? pageInfo,
        List<TItem> results,
        bool pagingWithAfter)
    {
        if (pageInfo is null)
        {
            return new GraphQlRelayPage<TItem> { Results = results };
        }

        bool hasPreviousPage = GraphQlRelayPageInfo.ApplyForwardPagingRules(
            pageInfo.HasPreviousPage,
            pagingWithAfter);

        return new GraphQlRelayPage<TItem>
        {
            Results = results,
            EndCursor = pageInfo.EndCursor,
            StartCursor = pageInfo.StartCursor,
            HasNextPage = pageInfo.HasNextPage,
            HasPreviousPage = hasPreviousPage,
        };
    }

    private static AccountBill MapBill(AccountBillWire wire)
    {
        return new AccountBill
        {
            Id = wire.Id,
            BillType = wire.BillType,
            IssuedDate = ParseDate(wire.IssuedDate),
            TemporaryUrl = wire.TemporaryUrl,
        };
    }

    private static AccountTransaction MapTransaction(AccountTransactionWire wire)
    {
        TransactionAmounts? amounts = wire.Amounts is null
            ? null
            : new TransactionAmounts
            {
                Net = wire.Amounts.Net,
                Tax = wire.Amounts.Tax,
                Gross = wire.Amounts.Gross,
            };

        return new AccountTransaction
        {
            Id = wire.Id,
            PostedDate = ParseDate(wire.PostedDate),
            KrakenType = wire.TypeName,
            Amounts = amounts,
        };
    }

    private static AccountPayment MapPayment(AccountPaymentWire wire)
    {
        return new AccountPayment
        {
            Id = wire.Id,
            Amount = wire.Amount,
            PaymentDate = ParseDate(wire.PaymentDate),
        };
    }

    private static DateOnly? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsed)
            ? parsed
            : null;
    }
}
