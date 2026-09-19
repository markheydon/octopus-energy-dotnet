using System.Net.Http.Headers;
using OctopusEnergy.Client.Infrastructure.Authentication;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal static class OctopusEnergyRequestHeaders
{
    internal static void Apply(HttpRequestMessage request, string? apiKey)
    {
        ArgumentNullException.ThrowIfNull(request);

        request.Headers.TryAddWithoutValidation("User-Agent", OctopusEnergyUserAgent.Value);

        if (!request.Headers.Accept.Any(mediaType =>
                string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase)))
        {
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (apiKey is not null && request.Headers.Authorization is null)
        {
            request.Headers.Authorization = BasicApiKeyHeader.Create(apiKey);
        }
    }
}
