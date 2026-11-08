# Account bills, transactions, and payments

List billing statements, ledger transactions, and payments through GraphQL using the same dashboard API key as REST.

## When to use this guide

You need PDF bill links, ledger history, or payment dates without authoring GraphQL documents yourself. The SDK issues **three separate queries** (bills, transactions, payments) so each request stays under the Kraken complexity cap (200).

## Prerequisites

- Dashboard API key — see [authentication](authentication.md)
- Account number — see [accounts](accounts.md) or `client.Viewer` when you do not have one yet

## List bills (one page)

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Common;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

GraphQlRelayPage<AccountBill> page = await client.AccountBilling.ListBillsPageAsync(
    "A-12345678",
    new AccountBillListRequest { First = 10, IncludeBillsWithoutPdf = true },
    cancellationToken);

foreach (AccountBill bill in page.Results)
{
    Console.WriteLine($"{bill.IssuedDate} {bill.BillType} {bill.TemporaryUrl}");
}

string? nextCursor = page.HasNextPage ? page.EndCursor : null;
```

## List transactions and payments

```csharp
GraphQlRelayPage<AccountTransaction> transactions = await client.AccountBilling
    .ListTransactionsPageAsync(
        "A-12345678",
        new AccountTransactionListRequest
        {
            First = 25,
            FromDate = new DateOnly(2026, 1, 1),
        },
        cancellationToken);

GraphQlRelayPage<AccountPayment> payments = await client.AccountBilling.ListPaymentsPageAsync(
    "A-12345678",
    cancellationToken: cancellationToken);
```

Amounts on transactions and payments are in **pence**. Transaction `KrakenType` comes from GraphQL `__typename` (for example `Payment`, `Charge`).

## Paging

Pass `After` on the list request with `page.EndCursor` from the previous page. Validate `first` with `GraphQlRelayLimits.ValidateFirst` rules (see [pagination](pagination.md)). When paging forward with `after`, treat `hasPreviousPage` as documented in `GraphQlRelayPageInfo`.

## Errors

GraphQL failures throw `OctopusEnergyKrakenException` subtypes (for example complexity `KT-CT-1188`). See [error handling](error-handling.md).

When Kraken returns HTTP 200 but `account` is null (unknown account number or no access for this API key), the SDK throws `OctopusEnergyParseException` instead of an empty page.
