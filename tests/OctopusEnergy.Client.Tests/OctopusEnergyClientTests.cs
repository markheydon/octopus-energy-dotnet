using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Authentication;
using OctopusEnergy.Client.Infrastructure.Http;
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
    public void Constructor_WithHttpClientWithoutBaseAddress_DoesNotMutateSuppliedClient()
    {
        using HttpClient httpClient = new();
        using OctopusEnergyClient client = new(httpClient);

        Assert.Null(httpClient.BaseAddress);
    }

    [Fact]
    public async Task Constructor_WithHttpClientWithoutBaseAddress_UsesDefaultBaseUrlOnRequests()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler);
        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.Equal(new Uri("https://api.octopus.energy/v1/items/"), request.RequestUri);
    }

    [Fact]
    public async Task Constructor_WithHttpClientWithoutAcceptHeader_AddsJsonAcceptHeaderPerRequest()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        Assert.Empty(httpClient.DefaultRequestHeaders.Accept);

        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.Contains(
            request.Headers.Accept,
            mediaType => string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase));
        Assert.Empty(httpClient.DefaultRequestHeaders.Accept);
    }

    [Fact]
    public void Constructor_WithHttpClientWithoutTrailingSlash_DoesNotMutateSuppliedBaseAddress()
    {
        Uri originalBaseAddress = new("https://api.example.test/v1");

        using HttpClient httpClient = new()
        {
            BaseAddress = originalBaseAddress,
        };
        using OctopusEnergyClient client = new(httpClient);

        Assert.Equal(originalBaseAddress, httpClient.BaseAddress);
    }

    [Fact]
    public async Task Constructor_WithHttpClientWithoutTrailingSlash_NormalizesRequestUri()
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
    public void Constructor_WithApiKeyAndHttpClientWithoutBaseAddress_DoesNotMutateSuppliedClient()
    {
        const string apiKey = "test-api-key-value";

        using HttpClient httpClient = new();
        using OctopusEnergyClient client = new(apiKey, httpClient);

        Assert.Null(httpClient.BaseAddress);
        Assert.Null(httpClient.DefaultRequestHeaders.Authorization);
    }

    [Fact]
    public async Task Constructor_WithApiKeyAndHttpClient_DoesNotReplaceDefaultAuthorizationHeader()
    {
        const string apiKey = "test-api-key-value";

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "existing-token");

        using OctopusEnergyClient client = new(apiKey, httpClient);

        Assert.Equal("Bearer", httpClient.DefaultRequestHeaders.Authorization?.Scheme);
        Assert.Equal("existing-token", httpClient.DefaultRequestHeaders.Authorization?.Parameter);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.NotNull(request.Headers.Authorization);
        Assert.Equal("Basic", request.Headers.Authorization.Scheme);
        Assert.Equal(
            BasicApiKeyHeader.Create(apiKey).Parameter,
            request.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task GetAsync_WhenSharedHttpClientAndDifferentApiKeys_SendsMatchingAuthorizationHeaders()
    {
        const string firstApiKey = "first-api-key";
        const string secondApiKey = "second-api-key";

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient sharedHttpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient firstClient = new(firstApiKey, sharedHttpClient);
        using OctopusEnergyClient secondClient = new(secondApiKey, sharedHttpClient);

        await firstClient.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);
        await secondClient.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        Assert.Equal(2, handler.SentRequests.Count);
        Assert.Null(sharedHttpClient.DefaultRequestHeaders.Authorization);
        Assert.Equal(
            BasicApiKeyHeader.Create(firstApiKey).Parameter,
            handler.SentRequests[0].Headers.Authorization?.Parameter);
        Assert.Equal(
            BasicApiKeyHeader.Create(secondApiKey).Parameter,
            handler.SentRequests[1].Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_WhenHttpClientDefaultAcceptIncludesJson_DoesNotDuplicateJsonAcceptHeader()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using HttpClient httpClient = new(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(handler.SentRequests);
        Assert.Single(
            request.Headers.Accept,
            mediaType => string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAsync_WhenHandlerCarriesAuthAndAnonymousClient_SendsAuthorizationHeader()
    {
        const string apiKey = "handler-api-key";

        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using OctopusEnergyClientHandler sdkHandler = new(apiKey)
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(sdkHandler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        using OctopusEnergyClient client = new(httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(innerHandler.SentRequests);
        Assert.Equal(
            BasicApiKeyHeader.Create(apiKey).Parameter,
            request.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_WhenHandlerAndClientApiKeysDiffer_ClientApiKeyTakesPrecedence()
    {
        const string handlerApiKey = "handler-api-key";
        const string clientApiKey = "client-api-key";

        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using OctopusEnergyClientHandler sdkHandler = new(handlerApiKey)
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(sdkHandler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        using OctopusEnergyClient client = new(clientApiKey, httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(innerHandler.SentRequests);
        Assert.Equal(
            BasicApiKeyHeader.Create(clientApiKey).Parameter,
            request.Headers.Authorization?.Parameter);
    }

    [Fact]
    public async Task GetAsync_WhenHandlerAndClientBothConfigured_DoesNotDuplicateUserAgentHeader()
    {
        const string apiKey = "test-api-key";

        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, """{"count":0,"next":null,"results":[]}""");

        using OctopusEnergyClientHandler sdkHandler = new(apiKey)
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(sdkHandler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
        using OctopusEnergyClient client = new(apiKey, httpClient);

        await client.Rest.GetAsync<PaginatedResponseStub>("items/", CancellationToken.None);

        HttpRequestMessage request = Assert.Single(innerHandler.SentRequests);
        Assert.Single(
            request.Headers.UserAgent,
            value => value.Product?.Name == OctopusEnergyUserAgent.ProductName);
    }

    [Fact]
    public void Constructor_WithSharedHttpClient_DoesNotLeakAuthorizationAcrossInstances()
    {
        const string firstApiKey = "first-api-key";
        const string secondApiKey = "second-api-key";

        using HttpClient sharedHttpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient firstClient = new(firstApiKey, sharedHttpClient);
        using OctopusEnergyClient secondClient = new(secondApiKey, sharedHttpClient);

        Assert.Null(sharedHttpClient.DefaultRequestHeaders.Authorization);
        Assert.NotNull(firstClient);
        Assert.NotNull(secondClient);
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
    public async Task GetAsync_WhenAnonymousClient_SendsSdkUserAgentHeader()
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
        Assert.Contains(request.Headers.UserAgent, value => value.Product?.Name == OctopusEnergyUserAgent.ProductName);
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
        Assert.Contains(request.Headers.UserAgent, value => value.Product?.Name == OctopusEnergyUserAgent.ProductName);
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
