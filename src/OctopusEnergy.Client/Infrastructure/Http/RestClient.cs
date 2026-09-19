using System.Net;
using System.Runtime.CompilerServices;
using System.Text.Json;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Common;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal sealed class RestClient
{
    internal const int DefaultMaxPageHops = 10_000;

    private readonly HttpClient _httpClient;
    private readonly Uri _baseAddress;
    private readonly string? _apiKey;
    private readonly int _maxPageHops;

    internal RestClient(HttpClient httpClient, Uri baseAddress, string? apiKey = null, int maxPageHops = DefaultMaxPageHops)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(baseAddress);

        if (maxPageHops < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxPageHops), maxPageHops, "Page hop limit must be at least 1.");
        }

        _httpClient = httpClient;
        _baseAddress = HttpClientConfiguration.NormalizeBaseAddress(baseAddress);
        _apiKey = apiKey;
        _maxPageHops = maxPageHops;
    }

    internal async Task<T> GetAsync<T>(string relativePath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        using HttpRequestMessage request = CreateGetRequest(relativePath);
        OctopusEnergyRequestHeaders.Apply(request, _apiKey);
        using HttpResponseMessage response = await SendAsync(request, cancellationToken).ConfigureAwait(false);
        await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            return await JsonSerializer.DeserializeAsync<T>(
                contentStream,
                OctopusJsonSerializerOptions.Default,
                cancellationToken).ConfigureAwait(false)
                ?? throw new OctopusEnergyException("The API returned an empty response body.");
        }
        catch (JsonException ex)
        {
            throw new OctopusEnergyException("The API returned a response that could not be deserialised.", ex);
        }
    }

    /// <summary>
    /// Fetches a single REST list page without following <c>next</c>.
    /// </summary>
    internal async Task<PaginatedResult<TItem>> GetPageAsync<TItem>(
        string relativePath,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        PaginatedResponse<TItem> page = await GetAsync<PaginatedResponse<TItem>>(relativePath, cancellationToken)
            .ConfigureAwait(false);

        return ToPaginatedResult(page);
    }

    /// <summary>
    /// Enumerates all items across REST pages, following <c>next</c> until it is null.
    /// Non-success HTTP responses throw; partial pages are not returned after a failure.
    /// </summary>
    internal async IAsyncEnumerable<TItem> GetAllPagesAsync<TItem>(
        string relativePath,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

        string? nextPath = relativePath;
        HashSet<string> visitedPaths = new(StringComparer.Ordinal);
        int pageHop = 0;

        while (nextPath is not null)
        {
            if (!visitedPaths.Add(nextPath))
            {
                throw new OctopusEnergyException(
                    "Pagination stopped because the API returned a repeated next URL.");
            }

            pageHop++;
            if (pageHop > _maxPageHops)
            {
                throw new OctopusEnergyException(
                    $"Pagination stopped after {_maxPageHops} pages. The next URL may be malformed.");
            }

            PaginatedResult<TItem> page = await GetPageAsync<TItem>(nextPath, cancellationToken)
                .ConfigureAwait(false);

            foreach (TItem item in page.Results)
            {
                yield return item;
            }

            nextPath = ResolveNextPath(page.Next);
        }
    }

    private static PaginatedResult<TItem> ToPaginatedResult<TItem>(PaginatedResponse<TItem> page)
    {
        return new PaginatedResult<TItem>
        {
            Count = page.Count,
            Next = page.Next,
            Previous = page.Previous,
            Results = page.Results ?? [],
        };
    }

    private HttpRequestMessage CreateGetRequest(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out Uri? absoluteUri))
        {
            return new HttpRequestMessage(HttpMethod.Get, absoluteUri);
        }

        Uri requestUri = new(_baseAddress, path);
        return new HttpRequestMessage(HttpMethod.Get, requestUri);
    }

    private string? ResolveNextPath(string? next)
    {
        if (string.IsNullOrWhiteSpace(next))
        {
            return null;
        }

        if (Uri.TryCreate(next, UriKind.Absolute, out Uri? absoluteUri))
        {
            if (absoluteUri.IsAbsoluteUri &&
                _baseAddress.IsAbsoluteUri &&
                string.Equals(absoluteUri.Host, _baseAddress.Host, StringComparison.OrdinalIgnoreCase))
            {
                string basePath = _baseAddress.AbsolutePath.TrimEnd('/');
                string nextPath = absoluteUri.AbsolutePath;

                if (basePath.Length > 0 &&
                    nextPath.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                {
                    nextPath = nextPath[basePath.Length..];
                }

                string relative = nextPath.TrimStart('/');
                if (!string.IsNullOrEmpty(absoluteUri.Query))
                {
                    relative += absoluteUri.Query;
                }

                return relative;
            }

            return absoluteUri.ToString();
        }

        return next;
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            await ThrowForResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }

    private static async Task ThrowForResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        using (response)
        {
            HttpStatusCode statusCode = response.StatusCode;
            string body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    ApiErrorResponse? error = JsonSerializer.Deserialize<ApiErrorResponse>(
                        body,
                        OctopusJsonSerializerOptions.Default);

                    if (!string.IsNullOrWhiteSpace(error?.Detail))
                    {
                        throw new OctopusEnergyApiException(statusCode, error.Detail);
                    }
                }
                catch (JsonException)
                {
                }
                catch (OctopusEnergyApiException)
                {
                    throw;
                }
            }

            throw new OctopusEnergyHttpException(
                statusCode,
                $"The Octopus API returned HTTP {(int)statusCode} ({statusCode}).");
        }
    }
}
