using OctopusEnergy.Client.Infrastructure.Http;

namespace OctopusEnergy.Client;

/// <summary>
/// Customer-facing client for the public Octopus Energy (Kraken) APIs.
/// </summary>
/// <remarks>
/// Resource methods will be added in later v1 stories. This client provides
/// the shared REST transport, pagination, and error handling used by those services.
/// </remarks>
public sealed class OctopusEnergyClient : IDisposable
{
    /// <summary>
    /// Default UK REST API base URL.
    /// </summary>
    public const string DefaultBaseUrl = "https://api.octopus.energy/v1/";

    private readonly HttpClient _httpClient;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Creates a client with the default UK API base URL.
    /// </summary>
    public OctopusEnergyClient()
        : this(CreateDefaultHttpClient(), ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Creates a client that uses the supplied <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// The HTTP client to use. When <see cref="HttpClient.BaseAddress"/> is null,
    /// <see cref="DefaultBaseUrl"/> is applied on the supplied instance. When no
    /// <c>Accept: application/json</c> header is present, one is added.
    /// </param>
    public OctopusEnergyClient(HttpClient httpClient)
        : this(httpClient, ownsHttpClient: false)
    {
    }

    private OctopusEnergyClient(HttpClient httpClient, bool ownsHttpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _ownsHttpClient = ownsHttpClient;

        if (_httpClient.BaseAddress is null)
        {
            _httpClient.BaseAddress = new Uri(DefaultBaseUrl);
        }

        EnsureJsonAcceptHeader(_httpClient);

        Rest = new RestClient(_httpClient);
    }

    internal RestClient Rest { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(DefaultBaseUrl),
        };

        EnsureJsonAcceptHeader(httpClient);

        return httpClient;
    }

    private static void EnsureJsonAcceptHeader(HttpClient httpClient)
    {
        if (httpClient.DefaultRequestHeaders.Accept.Any(mediaType =>
                string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }
}
