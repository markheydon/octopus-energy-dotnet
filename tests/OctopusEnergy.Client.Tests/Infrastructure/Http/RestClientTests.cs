using System.Net;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Infrastructure.Http;

public sealed class RestClientTests
{
    [Fact]
    public async Task GetAllPagesAsync_WhenTwoPages_ReturnsAllItems()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TestItem> items = await CollectAsync(client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None));

        Assert.Equal(["ITEM-1", "ITEM-2", "ITEM-3"], items.Select(item => item.Code));
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenNextIsNull_StopsAfterFirstPage()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TestItem> items = await CollectAsync(client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None));

        Assert.Single(items);
        Assert.Equal("ITEM-3", items[0].Code);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenNextRepeats_ThrowsOctopusEnergyException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-cycle.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-cycle.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyException exception = await Assert.ThrowsAsync<OctopusEnergyException>(
            async () => await CollectAsync(client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None)));

        Assert.Contains("repeated next URL", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("api-error-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("missing/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Not found.", exception.Detail);
    }

    [Fact]
    public async Task GetAsync_WhenServerError_ThrowsOctopusEnergyHttpException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.InternalServerError, "upstream failure");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("broken/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task GetAsync_WhenSuccessBodyIsInvalidJson_ThrowsOctopusEnergyException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, "not json");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyException exception = await Assert.ThrowsAsync<OctopusEnergyException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Contains("could not be deserialised", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_WhenErrorBodyHasNoDetail_ThrowsOctopusEnergyHttpException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.BadRequest, FixtureFile.Read("api-error-empty-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.BadRequest, exception.StatusCode);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenSecondPageFails_ThrowsAfterFirstPageItemsWereYielded()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-1.json"));
        handler.Enqueue(HttpStatusCode.InternalServerError, "upstream failure");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        List<TestItem> items = [];
        await using IAsyncEnumerator<TestItem> enumerator = client.Rest
            .GetAllPagesAsync<TestItem>("items/", cancellationToken)
            .GetAsyncEnumerator(cancellationToken);

        Assert.True(await enumerator.MoveNextAsync());
        items.Add(enumerator.Current);
        Assert.True(await enumerator.MoveNextAsync());
        items.Add(enumerator.Current);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            async () => await enumerator.MoveNextAsync());

        Assert.Equal(["ITEM-1", "ITEM-2"], items.Select(item => item.Code));
        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenPageHopLimitExceeded_ThrowsOctopusEnergyException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":1,"next":"items/?page=2","results":[{"code":"A"}]}""");
        handler.Enqueue(HttpStatusCode.OK, """{"count":1,"next":"items/?page=3","results":[{"code":"B"}]}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        RestClient rest = new(httpClient, new Uri("https://api.example.test/v1/"), maxPageHops: 2);

        OctopusEnergyException exception = await Assert.ThrowsAsync<OctopusEnergyException>(
            async () => await CollectAsync(rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None)));

        Assert.Contains("2 pages", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenResultsIsNull_ReturnsNoItems()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-null-results.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TestItem> items = await CollectAsync(client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None));

        Assert.Empty(items);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenNextIsRelative_FollowsLink()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-relative-next-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-relative-next-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TestItem> items = await CollectAsync(client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None));

        Assert.Equal(["REL-1", "REL-2"], items.Select(item => item.Code));
    }

    private static HttpClient CreateHttpClient(QueuedHttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
    }

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        List<T> items = [];
        await foreach (T item in source)
        {
            items.Add(item);
        }

        return items;
    }

    private sealed class TestItem
    {
        [JsonPropertyName("code")]
        public string Code { get; init; } = string.Empty;
    }

    private sealed class PaginatedResponseStub
    {
        [JsonPropertyName("count")]
        public int Count { get; init; }

        [JsonPropertyName("next")]
        public string? Next { get; init; }

        [JsonPropertyName("previous")]
        public string? Previous { get; init; }

        [JsonPropertyName("results")]
        public IReadOnlyList<TestItem> Results { get; init; } = [];
    }
}
