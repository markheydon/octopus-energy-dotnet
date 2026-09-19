using System.Net;
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

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);
        using CancellationTokenSource cancellationTokenSource = new();
        cancellationTokenSource.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.Rest.GetAsync<PaginatedResponseStub>("items/", cancellationTokenSource.Token));
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
