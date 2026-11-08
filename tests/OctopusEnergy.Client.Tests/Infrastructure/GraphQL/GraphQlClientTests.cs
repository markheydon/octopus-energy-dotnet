using System.Net;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class GraphQlClientTests
{
    private const string RateLimitInfoQuery = """
        query RateLimitInfo {
          rateLimitInfo {
            __typename
          }
        }
        """;

    [Fact]
    public async Task ExecuteAsync_WhenKtCt1112_RefreshesAndRetriesOnce()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1112.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-refresh.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-rate-limit-info.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, _, GraphQlClient graphQlClient) = GraphQlTestSupport.CreateStack(httpClient);

        RateLimitInfoData data = await graphQlClient.ExecuteAsync<RateLimitInfoData>(
            RateLimitInfoQuery,
            variables: null,
            CancellationToken.None);

        Assert.NotNull(data.RateLimitInfo);
        Assert.Equal(4, handler.SentRequests.Count);

        HttpRequestMessage authenticatedRetry = handler.SentRequests[3];
        AssertAuthorizationIsRawToken(authenticatedRetry, "eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJleHAiOjQ3MTAwMDAwMDB9.test-refreshed-access-signature");
    }

    [Fact]
    public async Task ExecuteAsync_WhenKtCt1112PersistsAfterRefresh_Throws()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1112.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-refresh.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1112.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, _, GraphQlClient graphQlClient) = GraphQlTestSupport.CreateStack(httpClient);

        OctopusEnergyKrakenException exception = await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => graphQlClient.ExecuteAsync<RateLimitInfoData>(
                RateLimitInfoQuery,
                variables: null,
                CancellationToken.None));

        Assert.Equal("KT-CT-1112", exception.KrakenErrorCode);
        Assert.Equal(4, handler.SentRequests.Count);
    }

    [Fact]
    public void Constructor_WhenApiKeyPresent_ExposesInternalGraphQlClient()
    {
        using OctopusEnergyClient client = new("test-api-key");

        Assert.NotNull(client.GraphQl);
    }

    [Fact]
    public void Constructor_WhenNoApiKey_GraphQlClientIsNull()
    {
        using OctopusEnergyClient client = new();

        Assert.Null(client.GraphQl);
    }

    private static void AssertAuthorizationIsRawToken(HttpRequestMessage request, string expectedToken)
    {
        Assert.True(request.Headers.TryGetValues("Authorization", out IEnumerable<string>? values));
        Assert.Equal(expectedToken, Assert.Single(values));
    }

    private sealed class RateLimitInfoData
    {
        [JsonPropertyName("rateLimitInfo")]
        public RateLimitInfoStub? RateLimitInfo { get; init; }
    }

    private sealed class RateLimitInfoStub
    {
        [JsonPropertyName("__typename")]
        public string? TypeName { get; init; }
    }
}
