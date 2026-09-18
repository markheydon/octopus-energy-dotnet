using OctopusEnergy.Client.Infrastructure.Authentication;
using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.Products;

namespace OctopusEnergy.Client;

/// <summary>
/// Customer-facing client for the public Octopus Energy (Kraken) APIs.
/// </summary>
/// <remarks>
/// Resource services use the shared REST transport, pagination, and error handling
/// implemented by this client.
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
    /// <remarks>
    /// Public catalogue endpoints work without authentication. Account and consumption
    /// calls require an API key; use the <see cref="OctopusEnergyClient(string)"/> overload.
    /// </remarks>
    public OctopusEnergyClient()
        : this(CreateDefaultHttpClient(), ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Creates an authenticated client with the default UK API base URL.
    /// </summary>
    /// <param name="apiKey">
    /// Dashboard API key. Sent as HTTP Basic authentication with an empty password.
    /// Treat as a secret; the SDK never logs it.
    /// </param>
    public OctopusEnergyClient(string apiKey)
        : this(ValidateApiKey(apiKey), new Uri(DefaultBaseUrl))
    {
    }

    /// <summary>
    /// Creates an authenticated client with a custom API base URL.
    /// </summary>
    /// <param name="apiKey">
    /// Dashboard API key. Sent as HTTP Basic authentication with an empty password.
    /// Treat as a secret; the SDK never logs it.
    /// </param>
    /// <param name="baseAddress">
    /// REST API base URL. Must be an absolute URI. A trailing slash is applied when missing.
    /// </param>
    public OctopusEnergyClient(string apiKey, Uri baseAddress)
        : this(ValidateApiKey(apiKey), CreateHttpClient(baseAddress), ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Creates a client that uses the supplied <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// The HTTP client to use. When <see cref="HttpClient.BaseAddress"/> is null,
    /// <see cref="DefaultBaseUrl"/> is applied on the supplied instance. When a base address
    /// is already set, a trailing slash is applied when missing. When no
    /// <c>Accept: application/json</c> header is present, one is added.
    /// </param>
    /// <remarks>
    /// Public catalogue endpoints work without authentication. No <c>Authorization</c> header
    /// is added unless an API key constructor is used.
    /// </remarks>
    public OctopusEnergyClient(HttpClient httpClient)
        : this(httpClient, ownsHttpClient: false)
    {
    }

    /// <summary>
    /// Creates an authenticated client that uses the supplied <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="apiKey">
    /// Dashboard API key. Sent as HTTP Basic authentication with an empty password.
    /// Treat as a secret; the SDK never logs it.
    /// </param>
    /// <param name="httpClient">
    /// The HTTP client to use. When <see cref="HttpClient.BaseAddress"/> is null,
    /// <see cref="DefaultBaseUrl"/> is applied on the supplied instance. When a base address
    /// is already set, a trailing slash is applied when missing. When no
    /// <c>Accept: application/json</c> header is present, one is added. Any existing
    /// <c>Authorization</c> header is replaced with HTTP Basic for the API key.
    /// </param>
    public OctopusEnergyClient(string apiKey, HttpClient httpClient)
        : this(ValidateApiKey(apiKey), httpClient, ownsHttpClient: false)
    {
    }

    private OctopusEnergyClient(string apiKey, HttpClient httpClient, bool ownsHttpClient)
        : this(httpClient, ownsHttpClient, apiKey)
    {
    }

    private static string ValidateApiKey(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return apiKey;
    }

    private OctopusEnergyClient(HttpClient httpClient, bool ownsHttpClient, string? apiKey = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _ownsHttpClient = ownsHttpClient;

        HttpClientConfiguration.ApplyBaseAddress(_httpClient, baseAddress: null, DefaultBaseUrl);
        EnsureJsonAcceptHeader(_httpClient);

        if (apiKey is not null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = BasicApiKeyHeader.Create(apiKey);
        }

        Rest = new RestClient(_httpClient);
        Accounts = new AccountService(Rest);
        Industry = new IndustryService(Rest);
        Products = new ProductService(Rest);
        TariffRates = new TariffRatesService(Rest);
    }

    /// <summary>
    /// Customer account detail.
    /// </summary>
    public AccountService Accounts { get; }

    /// <summary>
    /// Public industry lookups (postcode GSP and MPAN metadata).
    /// </summary>
    public IndustryService Industry { get; }

    /// <summary>
    /// Product catalogue and product detail.
    /// </summary>
    public ProductService Products { get; }

    /// <summary>
    /// Standing charges and unit rates for product tariffs.
    /// </summary>
    public TariffRatesService TariffRates { get; }

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
        return CreateHttpClient(new Uri(DefaultBaseUrl));
    }

    private static HttpClient CreateHttpClient(Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);

        HttpClient httpClient = new();
        HttpClientConfiguration.ApplyBaseAddress(httpClient, baseAddress, DefaultBaseUrl);
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
