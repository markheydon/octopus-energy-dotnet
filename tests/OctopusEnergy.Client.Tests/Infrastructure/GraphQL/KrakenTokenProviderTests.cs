using System.Net;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class KrakenTokenProviderTests
{
    [Fact]
    public async Task GetAuthorizationHeaderValueAsync_WhenCalledTwice_UsesCachedTokenWithoutSecondObtain()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, KrakenTokenProvider tokenProvider, _) = GraphQlTestSupport.CreateStack(httpClient);

        string first = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);
        string second = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);

        Assert.Equal(first, second);
        Assert.Single(handler.SentRequests);
        AssertObtainRequest(handler.SentRequests[0]);
        Assert.False(handler.SentRequests[0].Headers.Contains("Authorization"));
        Assert.True(first.Contains("test-access-signature", StringComparison.Ordinal));
        Assert.False(first.Contains(' ', StringComparison.Ordinal));
    }

    [Fact]
    public async Task GetAuthorizationHeaderValueAsync_WhenRefreshQueued_ReturnsRefreshedToken()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-refresh.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, KrakenTokenProvider tokenProvider, _) = GraphQlTestSupport.CreateStack(httpClient);

        _ = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);
        await tokenProvider.ForceRefreshAsync(CancellationToken.None);

        string refreshed = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);

        Assert.True(refreshed.Contains("test-refreshed-access-signature", StringComparison.Ordinal));
        Assert.Equal(2, handler.SentRequests.Count);
    }

    private static void AssertObtainRequest(HttpRequestMessage request)
    {
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.NotNull(request.RequestUri);
        Assert.Contains("/graphql/", request.RequestUri.AbsoluteUri, StringComparison.Ordinal);
    }

}
