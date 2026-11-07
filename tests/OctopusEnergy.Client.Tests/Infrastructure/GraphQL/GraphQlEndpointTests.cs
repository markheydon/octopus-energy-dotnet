using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.GraphQL;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class GraphQlEndpointTests
{
    [Fact]
    public void FromRestBaseAddress_WhenV1Root_AppendsGraphQlPath()
    {
        Uri rest = HttpClientConfiguration.NormalizeBaseAddress(new Uri("https://api.octopus.energy/v1/"));

        Uri graphQl = GraphQlEndpoint.FromRestBaseAddress(rest);

        Assert.Equal("https://api.octopus.energy/v1/graphql/", graphQl.AbsoluteUri);
    }
}
