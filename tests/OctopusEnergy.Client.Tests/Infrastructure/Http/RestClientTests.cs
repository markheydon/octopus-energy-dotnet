using System.Net;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
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
