using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.GraphQL;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

internal static class GraphQlTestSupport
{
    internal static (GraphQlHttpTransport Transport, KrakenTokenProvider TokenProvider, GraphQlClient Client) CreateStack(
        HttpClient httpClient)
    {
        Uri graphQlEndpoint = GraphQlEndpoint.FromRestBaseAddress(
            HttpClientConfiguration.NormalizeBaseAddress(new Uri("https://api.example.test/v1/")));

        GraphQlHttpTransport transport = new(httpClient, graphQlEndpoint);
        KrakenTokenProvider tokenProvider = new(transport, "test-api-key");
        GraphQlClient client = new(transport, tokenProvider);

        return (transport, tokenProvider, client);
    }
}
