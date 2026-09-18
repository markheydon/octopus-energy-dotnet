namespace OctopusEnergy.Client.Infrastructure.Configuration;

internal static class HttpClientConfiguration
{
    internal static Uri NormalizeBaseAddress(Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);

        if (!baseAddress.IsAbsoluteUri)
        {
            throw new ArgumentException("Base address must be an absolute URI.", nameof(baseAddress));
        }

        string absoluteUri = baseAddress.AbsoluteUri;

        if (!absoluteUri.EndsWith('/'))
        {
            absoluteUri += "/";
        }

        return new Uri(absoluteUri, UriKind.Absolute);
    }

    internal static void ApplyBaseAddress(HttpClient httpClient, Uri? baseAddress, string defaultBaseUrl)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        if (baseAddress is not null)
        {
            httpClient.BaseAddress = NormalizeBaseAddress(baseAddress);
            return;
        }

        if (httpClient.BaseAddress is null)
        {
            httpClient.BaseAddress = NormalizeBaseAddress(new Uri(defaultBaseUrl));
            return;
        }

        httpClient.BaseAddress = NormalizeBaseAddress(httpClient.BaseAddress);
    }
}
