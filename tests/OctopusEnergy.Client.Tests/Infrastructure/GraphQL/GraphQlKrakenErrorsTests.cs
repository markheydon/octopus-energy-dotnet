using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class GraphQlKrakenErrorsTests
{
    [Theory]
    [InlineData("graphql-error-kt-ct-1188.json", typeof(OctopusEnergyKrakenComplexityException), "KT-CT-1188")]
    [InlineData("graphql-error-kt-ct-1189.json", typeof(OctopusEnergyKrakenNodeLimitException), "KT-CT-1189")]
    [InlineData("graphql-error-kt-ct-1199.json", typeof(OctopusEnergyKrakenFieldRateLimitException), "KT-CT-1199")]
    public async Task ExecuteAsync_WhenKrakenError_ReturnsTypedException(
        string fixture,
        Type expectedType,
        string expectedCode)
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read(fixture));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        (_, _, GraphQlClient graphQlClient) = GraphQlTestSupport.CreateStack(httpClient);

        Exception exception = await Assert.ThrowsAnyAsync<Exception>(
            () => graphQlClient.ExecuteAsync<object>(
                "query { rateLimitInfo { __typename } }",
                variables: null,
                CancellationToken.None));

        Assert.IsType(expectedType, exception);
        OctopusEnergyKrakenException kraken = Assert.IsAssignableFrom<OctopusEnergyKrakenException>(exception);
        Assert.Equal(expectedCode, kraken.KrakenErrorCode);
    }
}
