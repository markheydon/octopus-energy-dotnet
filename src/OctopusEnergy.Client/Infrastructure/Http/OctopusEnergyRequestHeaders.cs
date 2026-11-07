using System.Net.Http.Headers;
using OctopusEnergy.Client.Infrastructure.Authentication;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal static class OctopusEnergyRequestHeaders
{
    internal static void Apply(HttpRequestMessage request, string? apiKey)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (!ContainsSdkUserAgent(request.Headers))
        {
            request.Headers.TryAddWithoutValidation("User-Agent", OctopusEnergyUserAgent.Value);
        }

        if (!HasJsonAcceptHeader(request.Headers))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (apiKey is not null && !HasAuthorizationHeader(request.Headers))
        {
            request.Headers.Authorization = BasicApiKeyHeader.Create(apiKey);
        }
    }

    internal static bool HasAuthorizationHeader(HttpRequestHeaders headers)
    {
        return headers.Authorization is not null
            || headers.Any(header => string.Equals(header.Key, "Authorization", StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasJsonAcceptHeader(HttpRequestHeaders? headers)
    {
        return headers?.Accept.Any(mediaType =>
            string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase)) == true;
    }

    private static bool ContainsSdkUserAgent(HttpRequestHeaders headers)
    {
        return headers.UserAgent.Any(value =>
            string.Equals(value.Product?.Name, OctopusEnergyUserAgent.ProductName, StringComparison.Ordinal));
    }
}
