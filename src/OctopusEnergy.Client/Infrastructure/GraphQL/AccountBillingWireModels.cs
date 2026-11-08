using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class AccountBillsQueryData
{
    [JsonPropertyName("account")]
    public AccountBillsAccountWire? Account { get; init; }
}

internal sealed class AccountBillsAccountWire
{
    [JsonPropertyName("bills")]
    public RelayConnectionWire<AccountBillWire>? Bills { get; init; }
}

internal sealed class AccountTransactionsQueryData
{
    [JsonPropertyName("account")]
    public AccountTransactionsAccountWire? Account { get; init; }
}

internal sealed class AccountTransactionsAccountWire
{
    [JsonPropertyName("transactions")]
    public RelayConnectionWire<AccountTransactionWire>? Transactions { get; init; }
}

internal sealed class AccountPaymentsQueryData
{
    [JsonPropertyName("account")]
    public AccountPaymentsAccountWire? Account { get; init; }
}

internal sealed class AccountPaymentsAccountWire
{
    [JsonPropertyName("payments")]
    public RelayConnectionWire<AccountPaymentWire>? Payments { get; init; }
}

internal sealed class RelayConnectionWire<TNode>
{
    [JsonPropertyName("edges")]
    public IReadOnlyList<RelayEdgeWire<TNode>>? Edges { get; init; }

    [JsonPropertyName("pageInfo")]
    public RelayPageInfoWire? PageInfo { get; init; }
}

internal sealed class RelayEdgeWire<TNode>
{
    [JsonPropertyName("node")]
    public TNode? Node { get; init; }

    [JsonPropertyName("cursor")]
    public string? Cursor { get; init; }
}

internal sealed class RelayPageInfoWire
{
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage { get; init; }

    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage { get; init; }

    [JsonPropertyName("endCursor")]
    public string? EndCursor { get; init; }

    [JsonPropertyName("startCursor")]
    public string? StartCursor { get; init; }
}

internal sealed class AccountBillWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("billType")]
    public string? BillType { get; init; }

    [JsonPropertyName("issuedDate")]
    public string? IssuedDate { get; init; }

    [JsonPropertyName("temporaryUrl")]
    public string? TemporaryUrl { get; init; }
}

internal sealed class AccountTransactionWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("postedDate")]
    public string? PostedDate { get; init; }

    [JsonPropertyName("__typename")]
    public string? TypeName { get; init; }

    [JsonPropertyName("amounts")]
    public TransactionAmountsWire? Amounts { get; init; }
}

internal sealed class TransactionAmountsWire
{
    [JsonPropertyName("net")]
    public int? Net { get; init; }

    [JsonPropertyName("tax")]
    public int? Tax { get; init; }

    [JsonPropertyName("gross")]
    public int? Gross { get; init; }
}

internal sealed class AccountPaymentWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("amount")]
    public int? Amount { get; init; }

    [JsonPropertyName("paymentDate")]
    public string? PaymentDate { get; init; }
}

internal sealed class AccountBillsQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("first")]
    public required int First { get; init; }

    [JsonPropertyName("after")]
    public string? After { get; init; }

    [JsonPropertyName("includeBillsWithoutPDF")]
    public required bool IncludeBillsWithoutPdf { get; init; }
}

internal sealed class AccountTransactionsQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("first")]
    public required int First { get; init; }

    [JsonPropertyName("after")]
    public string? After { get; init; }

    [JsonPropertyName("fromDate")]
    public string? FromDate { get; init; }

    [JsonPropertyName("toDate")]
    public string? ToDate { get; init; }
}

internal sealed class AccountPaymentsQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }

    [JsonPropertyName("first")]
    public required int First { get; init; }

    [JsonPropertyName("after")]
    public string? After { get; init; }
}
