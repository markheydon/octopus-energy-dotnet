using System.Net;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Authentication;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyClientTests
{
    [Fact]
    public void Constructor_Default_CreatesInstance()
    {
        using OctopusEnergyClient client = new();

        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithHttpClient_UsesSuppliedClient()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient client = new(httpClient);

        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithHttpClientWithoutBaseAddress_SetsDefaultBaseAddress()
    {
        using HttpClient httpClient = new();
        using OctopusEnergyClient client = new(httpClient);

        Assert.Equal(new Uri(OctopusEnergyClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void Constructor_WithHttpClientWithoutAcceptHeader_AddsJsonAcceptHeader()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient client = new(httpClient);

        Assert.Contains(
            httpClient.DefaultRequestHeaders.Accept,
            mediaType => string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Constructor_WithHttpClientWithoutTrailingSlash_NormalizesBaseAddress()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1"),
        };

        using OctopusEnergyClient client = new(httpClient);

        Assert.Equal(new Uri("https://api.example.test/v1/"), httpClient.BaseAddress);
    }

    [Fact]
    public void Constructor_WithApiKeyAndHttpClientWithoutBaseAddress_SetsDefaultBaseAddress()
    {
        const string apiKey = "test-api-key-value";

        using HttpClient httpClient = new();
        using OctopusEnergyClient client = new(apiKey, httpClient);

        Assert.Equal(new Uri(OctopusEnergyClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void Constructor_WithApiKeyAndHttpClient_ReplacesExistingAuthorizationHeader()
    {
        const string apiKey = "test-api-key-value";

        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "existing-token");

        using OctopusEnergyClient client = new(apiKey, httpClient);

        Assert.Equal("Basic", httpClient.DefaultRequestHeaders.Authorization?.Scheme);
        Assert.Equal(
            BasicApiKeyHeader.Create(apiKey).Parameter,
            httpClient.DefaultRequestHeaders.Authorization?.Parameter);
    }

    [Fact]
    public void Constructor_WithNullApiKey_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => new OctopusEnergyClient((string)null!));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithWhitespaceApiKey_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => new OctopusEnergyClient("   "));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNullBaseAddress_ThrowsArgumentNullException()
    {
        const string apiKey = "test-api-key-value";

        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => new OctopusEnergyClient(apiKey, (Uri)null!));

        Assert.Equal("baseAddress", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithRelativeBaseAddress_ThrowsArgumentException()
    {
        const string apiKey = "test-api-key-value";

        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => new OctopusEnergyClient(apiKey, new Uri("/v1/", UriKind.Relative)));

        Assert.Equal("baseAddress", exception.ParamName);
    }

    [Fact]
    public async Task GetAsync_WhenDefaultClient_DoesNotSendAuthorizationHeader()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.Null(request.Headers.Authorization);
    }

    [Fact]
    public async Task GetAsync_WhenHttpClientBaseAddressMissingTrailingSlash_ResolvesRelativePathCorrectly()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1"),
        };
        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.Equal(new Uri("https://api.example.test/v1/items/"), request.RequestUri);
    }

    [Fact]
    public async Task GetAsync_WhenApiKeyClient_SendsBasicAuthorizationHeader()
    {
        const string apiKey = "test-api-key-value";

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        using OctopusEnergyClient client = new(apiKey, httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("Basic", request.Headers.Authorization.Scheme);
        Assert.Equal(
            BasicApiKeyHeader.Create(apiKey).Parameter,
            request.Headers.Authorization.Parameter);
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
