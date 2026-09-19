using System.Net;
using System.Net.Http.Headers;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Authentication;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyClientHandlerTests
{
    [Fact]
    public async Task SendAsync_WhenAnonymous_AddsAcceptAndUserAgentHeaders()
    {
        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, "ok");

        using OctopusEnergyClientHandler handler = new()
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(handler);

        using HttpRequestMessage request = new(HttpMethod.Get, "https://api.example.test/v1/items/");
        using HttpResponseMessage response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpRequestMessage sentRequest = Assert.Single(innerHandler.SentRequests);
        Assert.Contains(
            sentRequest.Headers.Accept,
            mediaType => string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(sentRequest.Headers.UserAgent, value => value.Product?.Name == OctopusEnergyUserAgent.ProductName);
        Assert.Null(sentRequest.Headers.Authorization);
    }

    [Fact]
    public async Task SendAsync_WhenAuthenticated_AddsBasicAuthorizationHeader()
    {
        const string apiKey = "test-api-key-value";

        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, "ok");

        using OctopusEnergyClientHandler handler = new(apiKey)
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(handler);

        using HttpRequestMessage request = new(HttpMethod.Get, "https://api.example.test/v1/items/");
        using HttpResponseMessage response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpRequestMessage sentRequest = Assert.Single(innerHandler.SentRequests);
        Assert.NotNull(sentRequest.Headers.Authorization);
        Assert.Equal("Basic", sentRequest.Headers.Authorization.Scheme);
        Assert.Equal(
            BasicApiKeyHeader.Create(apiKey).Parameter,
            sentRequest.Headers.Authorization.Parameter);
    }

    [Fact]
    public async Task SendAsync_WhenRequestAlreadyHasAuthorization_DoesNotReplaceHeader()
    {
        const string apiKey = "test-api-key-value";

        QueuedHttpMessageHandler innerHandler = new();
        innerHandler.Enqueue(HttpStatusCode.OK, "ok");

        using OctopusEnergyClientHandler handler = new(apiKey)
        {
            InnerHandler = innerHandler,
        };
        using HttpClient httpClient = new(handler);

        using HttpRequestMessage request = new(HttpMethod.Get, "https://api.example.test/v1/items/");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "existing-token");
        using HttpResponseMessage response = await httpClient.SendAsync(request, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        HttpRequestMessage sentRequest = Assert.Single(innerHandler.SentRequests);
        Assert.Equal("Bearer", sentRequest.Headers.Authorization?.Scheme);
        Assert.Equal("existing-token", sentRequest.Headers.Authorization?.Parameter);
    }
}
