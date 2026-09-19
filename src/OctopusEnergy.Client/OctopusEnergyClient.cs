using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
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
public sealed class OctopusEnergyClient : IOctopusEnergyClient
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
    /// <param name="retryOptions">
    /// Optional retry policy for HTTP 429 and 503 responses. When omitted,
    /// <see cref="OctopusEnergyRetryOptions.Default"/> is used.
    /// </param>
    /// <remarks>
    /// Public catalogue endpoints work without authentication. Account and consumption
    /// calls require an API key; use the <see cref="OctopusEnergyClient(string, OctopusEnergyRetryOptions?)"/> overload.
    /// </remarks>
    public OctopusEnergyClient(OctopusEnergyRetryOptions? retryOptions = null)
        : this(CreateOwnedHttpClient(baseAddress: null), ownsHttpClient: true, apiKey: null, retryOptions)
    {
    }

    /// <summary>
    /// Creates an authenticated client with the default UK API base URL.
    /// </summary>
    /// <param name="apiKey">
    /// Dashboard API key. Sent as HTTP Basic authentication with an empty password.
    /// Treat as a secret; the SDK never logs it.
    /// </param>
    /// <param name="retryOptions">
    /// Optional retry policy for HTTP 429 and 503 responses. When omitted,
    /// <see cref="OctopusEnergyRetryOptions.Default"/> is used.
    /// </param>
    public OctopusEnergyClient(string apiKey, OctopusEnergyRetryOptions? retryOptions = null)
        : this(ValidateApiKey(apiKey), new Uri(DefaultBaseUrl), retryOptions)
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
    /// <param name="retryOptions">
    /// Optional retry policy for HTTP 429 and 503 responses. When omitted,
    /// <see cref="OctopusEnergyRetryOptions.Default"/> is used.
    /// </param>
    public OctopusEnergyClient(string apiKey, Uri baseAddress, OctopusEnergyRetryOptions? retryOptions = null)
        : this(ValidateApiKey(apiKey), CreateOwnedHttpClient(ValidateBaseAddress(baseAddress)), ownsHttpClient: true, retryOptions)
    {
    }

    /// <summary>
    /// Creates a client that uses the supplied <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient">
    /// The HTTP client to use. When <see cref="HttpClient.BaseAddress"/> is null, the SDK uses
    /// <see cref="DefaultBaseUrl"/> for relative REST paths without mutating the supplied instance.
    /// Authentication, <c>Accept</c>, and <c>User-Agent</c> are applied per request.
    /// </param>
    /// <param name="retryOptions">
    /// Optional retry policy for HTTP 429 and 503 responses. When omitted,
    /// <see cref="OctopusEnergyRetryOptions.Default"/> is used.
    /// </param>
    /// <remarks>
    /// Public catalogue endpoints work without authentication. No <c>Authorization</c> header
    /// is added unless an API key constructor is used.
    /// For <c>IHttpClientFactory</c>, register <see cref="OctopusEnergyClientHandler"/> on the
    /// named client and set <see cref="HttpClient.BaseAddress"/> at registration time.
    /// </remarks>
    public OctopusEnergyClient(HttpClient httpClient, OctopusEnergyRetryOptions? retryOptions = null)
        : this(httpClient, ownsHttpClient: false, apiKey: null, retryOptions)
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
    /// The HTTP client to use. When <see cref="HttpClient.BaseAddress"/> is null, the SDK uses
    /// <see cref="DefaultBaseUrl"/> for relative REST paths without mutating the supplied instance.
    /// Authentication, <c>Accept</c>, and <c>User-Agent</c> are applied per request and do not
    /// replace headers on <see cref="HttpClient.DefaultRequestHeaders"/>.
    /// When both this constructor and <see cref="OctopusEnergyClientHandler"/> supply an API key,
    /// the client key is applied first and the handler does not replace an existing
    /// <c>Authorization</c> header.
    /// </param>
    /// <param name="retryOptions">
    /// Optional retry policy for HTTP 429 and 503 responses. When omitted,
    /// <see cref="OctopusEnergyRetryOptions.Default"/> is used.
    /// </param>
    public OctopusEnergyClient(string apiKey, HttpClient httpClient, OctopusEnergyRetryOptions? retryOptions = null)
        : this(ValidateApiKey(apiKey), httpClient, ownsHttpClient: false, retryOptions)
    {
    }

    private OctopusEnergyClient(string apiKey, HttpClient httpClient, bool ownsHttpClient, OctopusEnergyRetryOptions? retryOptions = null)
        : this(httpClient, ownsHttpClient, apiKey, retryOptions)
    {
    }

    private static string ValidateApiKey(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return apiKey;
    }

    private static Uri ValidateBaseAddress(Uri baseAddress)
    {
        ArgumentNullException.ThrowIfNull(baseAddress);
        return baseAddress;
    }

    private OctopusEnergyClient(
        HttpClient httpClient,
        bool ownsHttpClient,
        string? apiKey = null,
        OctopusEnergyRetryOptions? retryOptions = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
        _ownsHttpClient = ownsHttpClient;

        Uri baseAddress = ResolveBaseAddress(httpClient.BaseAddress);

        Rest = new RestClient(_httpClient, baseAddress, apiKey, retryOptions: retryOptions);
        Accounts = new AccountService(Rest);
        Consumption = new ConsumptionService(Rest);
        Industry = new IndustryService(Rest);
        Products = new ProductService(Rest);
        TariffRates = new TariffRatesService(Rest);
    }

    /// <summary>
    /// Customer account detail.
    /// </summary>
    public IAccountService Accounts { get; }

    /// <summary>
    /// Electricity and gas consumption intervals.
    /// </summary>
    public IConsumptionService Consumption { get; }

    /// <summary>
    /// Public industry lookups (postcode GSP and MPAN metadata).
    /// </summary>
    public IIndustryService Industry { get; }

    /// <summary>
    /// Product catalogue and product detail.
    /// </summary>
    public IProductService Products { get; }

    /// <summary>
    /// Standing charges and unit rates for product tariffs.
    /// </summary>
    public ITariffRatesService TariffRates { get; }

    internal RestClient Rest { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

    private static HttpClient CreateOwnedHttpClient(Uri? baseAddress)
    {
        HttpClient httpClient = new(new HttpClientHandler(), disposeHandler: true);
        HttpClientConfiguration.ApplyBaseAddress(httpClient, baseAddress, DefaultBaseUrl);

        return httpClient;
    }

    private static Uri ResolveBaseAddress(Uri? httpClientBaseAddress)
    {
        if (httpClientBaseAddress is not null)
        {
            return HttpClientConfiguration.NormalizeBaseAddress(httpClientBaseAddress);
        }

        return HttpClientConfiguration.NormalizeBaseAddress(new Uri(DefaultBaseUrl));
    }
}
