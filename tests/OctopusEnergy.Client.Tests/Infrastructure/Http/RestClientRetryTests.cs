using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Infrastructure.Http;

public sealed class RestClientRetryTests
{
    [Fact]
    public async Task GetAsync_When429WithRetryAfter_ThenSucceeds_RetriesOnce()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResponseStub result = await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetAsync_When429ExhaustsAttempts_ThrowsWithLastStatus()
    {
        QueuedHttpMessageHandler handler = new();
        for (int attempt = 0; attempt < 4; attempt++)
        {
            handler.Enqueue(
                HttpStatusCode.TooManyRequests,
                "rate limited",
                response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));
        }

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Equal(4, handler.SentRequests.Count);
        Assert.DoesNotContain("sk_", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_When401_NotRetried()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.Unauthorized, FixtureFile.Read("api-error-empty-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_When503WithRetryAfter_ThenSucceeds()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.ServiceUnavailable,
            "temporarily unavailable",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResponseStub result = await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetAsync_WhenRetryDisabled_On429_ThrowsImmediately()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient, OctopusEnergyRetryOptions.Disabled);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_WhenCancelledDuringRetryDelay_ThrowsOperationCanceled()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "30"));
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        using CancellationTokenSource cancellationTokenSource = new();
        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        Task<PaginatedResponseStub> requestTask = client.Rest.GetAsync<PaginatedResponseStub>(
            "items/",
            cancellationTokenSource.Token);

        while (handler.SentRequests.Count < 1)
        {
            await Task.Delay(10, TestContext.Current.CancellationToken);
        }

        await cancellationTokenSource.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await requestTask);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_When429WithoutRetryAfter_UsesExponentialBackoff()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.TooManyRequests, "rate limited");
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        OctopusEnergyRetryOptions retryOptions = new()
        {
            BaseDelay = TimeSpan.FromMilliseconds(100),
            MaxDelay = TimeSpan.FromSeconds(5),
        };

        using HttpClient httpClient = CreateHttpClient(handler);
        RestClient rest = new(
            httpClient,
            new Uri("https://api.example.test/v1/"),
            retryOptions: retryOptions);

        DateTimeOffset started = DateTimeOffset.UtcNow;
        PaginatedResponseStub result = await rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);
        TimeSpan elapsed = DateTimeOffset.UtcNow - started;

        Assert.Equal(0, result.Count);
        Assert.Equal(2, handler.SentRequests.Count);
        Assert.True(elapsed >= TimeSpan.FromMilliseconds(80));
        Assert.True(elapsed < TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task GetAsync_When429WithRetryAfterHttpDate_ThenSucceeds()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.RetryAfter = new RetryConditionHeaderValue(DateTimeOffset.UtcNow));
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResponseStub result = await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        Assert.Equal(0, result.Count);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetAllPagesAsync_WhenSecondPage429_RetriesWithIndependentBudget()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-1.json"));
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TestItem> items = [];
        await foreach (TestItem item in client.Rest.GetAllPagesAsync<TestItem>("items/", CancellationToken.None))
        {
            items.Add(item);
        }

        Assert.Equal(["ITEM-1", "ITEM-2", "ITEM-3"], items.Select(item => item.Code));
        Assert.Equal(3, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetAsync_WhenRetryDisabledViaApiKeyConstructor_ThrowsImmediately()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "0"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new("sk_test_key", httpClient, OctopusEnergyRetryOptions.Disabled);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.TooManyRequests, exception.StatusCode);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_When429WithRetryAfter_RespectsMaxDelay()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.TooManyRequests,
            "rate limited",
            response => response.Headers.TryAddWithoutValidation("Retry-After", "3600"));
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"previous":null,"results":[]}""");

        OctopusEnergyRetryOptions retryOptions = new()
        {
            MaxDelay = TimeSpan.FromMilliseconds(1),
            BaseDelay = TimeSpan.FromMilliseconds(1),
        };

        using HttpClient httpClient = CreateHttpClient(handler);
        RestClient rest = new(
            httpClient,
            new Uri("https://api.example.test/v1/"),
            retryOptions: retryOptions);

        DateTimeOffset started = DateTimeOffset.UtcNow;
        PaginatedResponseStub result = await rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);
        TimeSpan elapsed = DateTimeOffset.UtcNow - started;

        Assert.Equal(0, result.Count);
        Assert.Equal(2, handler.SentRequests.Count);
        Assert.True(elapsed < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetAsync_When500_NotRetried()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.InternalServerError, "upstream failure");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyHttpException exception = await Assert.ThrowsAsync<OctopusEnergyHttpException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None));

        Assert.Equal(HttpStatusCode.InternalServerError, exception.StatusCode);
        Assert.Single(handler.SentRequests);
    }

    private static HttpClient CreateHttpClient(QueuedHttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
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
        public IReadOnlyList<object> Results { get; init; } = [];
    }
}
