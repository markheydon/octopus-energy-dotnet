using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Live;

/// <summary>
/// Opt-in probe that records the GraphQL <c>Authorization</c> header scheme against the live API.
/// </summary>
public sealed class LiveGraphQlTokenProbeTests
{
    private const string ViewerQuery = """
        query Viewer {
          viewer {
            id
          }
        }
        """;

    [Fact]
    public async Task ObtainKrakenTokenAndViewer_WhenLiveEnabled_RawAuthorizationHeaderSucceeds()
    {
        LiveTestGate.SkipUnlessEnabled();

        string? apiKey = Environment.GetEnvironmentVariable(LiveTestGate.ApiKeyEnvironmentVariable);
        Assert.False(string.IsNullOrWhiteSpace(apiKey), $"Set {LiveTestGate.ApiKeyEnvironmentVariable}.");

        using HttpClient httpClient = new()
        {
            BaseAddress = HttpClientConfiguration.NormalizeBaseAddress(new Uri(OctopusEnergyClient.DefaultBaseUrl)),
        };

        Uri graphQlEndpoint = GraphQlEndpoint.FromRestBaseAddress(httpClient.BaseAddress!);
        GraphQlHttpTransport transport = new(httpClient, graphQlEndpoint);
        KrakenTokenProvider tokenProvider = new(transport, apiKey!);
        GraphQlClient graphQl = new(transport, tokenProvider);

        string authorization = await tokenProvider.GetAuthorizationHeaderValueAsync(CancellationToken.None);

        await graphQl.ExecuteAsync<ViewerProbeData>(ViewerQuery, CancellationToken.None);

        Assert.False(authorization.Contains(' ', StringComparison.Ordinal));
    }

    private sealed class ViewerProbeData
    {
        [System.Text.Json.Serialization.JsonPropertyName("viewer")]
        public object? Viewer { get; init; }
    }
}
