using OctopusEnergy.Client.Infrastructure.Configuration;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class GraphQlEndpoint
{
    internal static Uri FromRestBaseAddress(Uri restBaseAddress)
    {
        ArgumentNullException.ThrowIfNull(restBaseAddress);

        string absolutePath = restBaseAddress.AbsolutePath;

        if (absolutePath.Contains("/graphql", StringComparison.OrdinalIgnoreCase))
        {
            return HttpClientConfiguration.NormalizeBaseAddress(restBaseAddress);
        }

        UriBuilder builder = new(restBaseAddress)
        {
            Path = absolutePath.TrimEnd('/') + "/graphql/",
        };

        return HttpClientConfiguration.NormalizeBaseAddress(builder.Uri);
    }
}
