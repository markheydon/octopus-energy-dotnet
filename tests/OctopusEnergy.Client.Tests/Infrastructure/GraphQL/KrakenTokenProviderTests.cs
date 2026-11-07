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
    public async Task GetAuthorizationHeaderValueAsync_WhenAccessTokenNearExpiry_CallsRefreshNotSecondObtain()
    {
        long nearExpiryUnix = DateTimeOffset.UtcNow.AddSeconds(90).ToUnixTimeSeconds();
        string nearExpiryToken = BuildJwtWithExpiryUnix(nearExpiryUnix, "near-expiry-access");
        string obtainNearExpiryJson = $$"""
            {
              "data": {
                "obtainKrakenToken": {
                  "token": "{{nearExpiryToken}}",
                  "refreshToken": "REFRESH-TOKEN-PLACEHOLDER",
                  "refreshExpiresIn": 2000000000
                }
              }
            }
            """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, obtainNearExpiryJson);
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-refresh.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, KrakenTokenProvider tokenProvider, _) = GraphQlTestSupport.CreateStack(httpClient);

        _ = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);
        string refreshed = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);

        Assert.True(refreshed.Contains("test-refreshed-access-signature", StringComparison.Ordinal));
        Assert.Equal(2, handler.SentRequests.Count);
        string secondBody = handler.SentRequestBodies[1];
        Assert.Contains("REFRESH-TOKEN-PLACEHOLDER", secondBody, StringComparison.Ordinal);
        Assert.DoesNotContain("test-api-key", secondBody, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ForceRefreshAsync_WhenRefreshOmitsRefreshExpiresIn_SubsequentForceRefreshObtainsWithApiKey()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-refresh-no-expiry.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, KrakenTokenProvider tokenProvider, _) = GraphQlTestSupport.CreateStack(httpClient);

        _ = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);
        string afterRefresh = await tokenProvider.ForceRefreshAsync(CancellationToken.None);
        string afterReObtain = await tokenProvider.ForceRefreshAsync(CancellationToken.None);

        Assert.Equal(3, handler.SentRequests.Count);
        Assert.Contains("test-refreshed-access-signature", afterRefresh, StringComparison.Ordinal);
        Assert.Contains("test-access-signature", afterReObtain, StringComparison.Ordinal);
        string thirdBody = handler.SentRequestBodies[2];
        Assert.Contains("test-api-key", thirdBody, StringComparison.Ordinal);
        Assert.DoesNotContain("REFRESH-TOKEN-ROTATED", thirdBody, StringComparison.Ordinal);
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

    private static string BuildJwtWithExpiryUnix(long expiryUnix, string signatureSuffix)
    {
        string payloadJson = $$"""{"exp":{{expiryUnix}}}""";
        string payloadSegment = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        return $"eyJhbGciOiJub25lIn0.{payloadSegment}.{signatureSuffix}";
    }

}
