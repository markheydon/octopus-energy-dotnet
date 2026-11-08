using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.AccountBilling;

public sealed class AccountBillingServiceTests
{
    private const string AccountNumber = "A-TEST0001";

    [Fact]
    public async Task ListBillsPageAsync_WhenSuccessful_ReturnsMappedPage()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-account-bills-page.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        GraphQlRelayPage<AccountBill> page = await client.AccountBilling.ListBillsPageAsync(
          AccountNumber,
          new AccountBillListRequest { First = 10 },
          CancellationToken.None);

        Assert.Single(page.Results);
        AccountBill bill = page.Results[0];
        Assert.Equal("bill-id-redacted", bill.Id);
        Assert.Equal("STATEMENT", bill.BillType);
        Assert.Equal(new DateOnly(2026, 10, 14), bill.IssuedDate);
        Assert.True(page.HasNextPage);
        Assert.Equal("cursor-bills-end", page.EndCursor);
        Assert.Equal(2, handler.SentRequests.Count);
        string requestBody = handler.SentRequestBodies[1];
        Assert.Contains("\"accountNumber\":\"A-TEST0001\"", requestBody, StringComparison.Ordinal);
        Assert.Contains("\"includeBillsWithoutPDF\":", requestBody, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListTransactionsPageAsync_WhenSuccessful_MapsKrakenType()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-account-transactions-page.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        GraphQlRelayPage<AccountTransaction> page = await client.AccountBilling.ListTransactionsPageAsync(
          AccountNumber,
          cancellationToken: CancellationToken.None);

        Assert.Single(page.Results);
        AccountTransaction transaction = page.Results[0];
        Assert.Equal("Payment", transaction.KrakenType);
        Assert.Equal(4000, transaction.Amounts?.Gross);
        Assert.False(page.HasNextPage);
        Assert.Contains("\"accountNumber\":\"A-TEST0001\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListPaymentsPageAsync_WhenEmpty_ReturnsEmptyResults()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-account-payments-empty.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        GraphQlRelayPage<AccountPayment> page = await client.AccountBilling.ListPaymentsPageAsync(
          AccountNumber,
          cancellationToken: CancellationToken.None);

        Assert.Empty(page.Results);
        Assert.False(page.HasNextPage);
    }

    [Fact]
    public async Task ListBillsPageAsync_WhenFirstExceedsMaximum_ThrowsRequestException()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
          () => client.AccountBilling.ListBillsPageAsync(
            AccountNumber,
            new AccountBillListRequest { First = 101 },
            CancellationToken.None));
    }

    [Fact]
    public async Task ListBillsPageAsync_WhenAccountNumberMissing_ThrowsRequestException()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
          () => client.AccountBilling.ListBillsPageAsync(" ", cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task ListBillsPageAsync_WhenNoApiKey_ThrowsRequestException()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
          () => client.AccountBilling.ListBillsPageAsync(AccountNumber, cancellationToken: CancellationToken.None));
    }

    [Theory]
    [InlineData("ListBillsPageAsync")]
    [InlineData("ListTransactionsPageAsync")]
    [InlineData("ListPaymentsPageAsync")]
    public async Task List_WhenAccountNull_ThrowsParseException(string operation)
    {
        const string body = """
      {
        "data": {
          "account": null
        },
        "errors": null
      }
      """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, body);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        Task task = operation switch
        {
            "ListBillsPageAsync" => client.AccountBilling.ListBillsPageAsync(AccountNumber, cancellationToken: CancellationToken.None),
            "ListTransactionsPageAsync" => client.AccountBilling.ListTransactionsPageAsync(AccountNumber, cancellationToken: CancellationToken.None),
            "ListPaymentsPageAsync" => client.AccountBilling.ListPaymentsPageAsync(AccountNumber, cancellationToken: CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };

        await Assert.ThrowsAsync<OctopusEnergyParseException>(() => task);
    }

    [Fact]
    public async Task ListBillsPageAsync_WhenKrakenError_ThrowsKrakenException()
    {
        const string body = """
      {
        "data": null,
        "errors": [
          {
            "message": "Query complexity exceeded.",
            "extensions": { "errorCode": "KT-CT-1188" }
          }
        ]
      }
      """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, body);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyKrakenComplexityException>(
          () => client.AccountBilling.ListBillsPageAsync(AccountNumber, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task ListBillsPageAsync_WhenPagingWithAfter_ClearsHasPreviousPage()
    {
        const string body = """
      {
        "data": {
          "account": {
            "bills": {
              "edges": [],
              "pageInfo": {
                "hasNextPage": false,
                "hasPreviousPage": true,
                "endCursor": null,
                "startCursor": null
              }
            }
          }
        },
        "errors": null
      }
      """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, body);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        GraphQlRelayPage<AccountBill> page = await client.AccountBilling.ListBillsPageAsync(
          AccountNumber,
          new AccountBillListRequest { After = "cursor-previous" },
          CancellationToken.None);

        Assert.False(page.HasPreviousPage);
    }
}
