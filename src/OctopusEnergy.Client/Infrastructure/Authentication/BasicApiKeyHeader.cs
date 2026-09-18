using System.Net.Http.Headers;
using System.Text;

namespace OctopusEnergy.Client.Infrastructure.Authentication;

internal static class BasicApiKeyHeader
{
    internal static AuthenticationHeaderValue Create(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        string credentials = $"{apiKey}:";
        string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));

        return new AuthenticationHeaderValue("Basic", encoded);
    }
}
