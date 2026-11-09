namespace OctopusEnergy.Client;

/// <summary>
/// Configuration for <c>AddOctopusEnergyClient</c> in <c>OctopusEnergy.Client.DependencyInjection</c>.
/// </summary>
public sealed class OctopusEnergyClientOptions
{
    /// <summary>
    /// Default named <see cref="HttpClient"/> registration name.
    /// </summary>
    public const string DefaultHttpClientName = "OctopusEnergy";

    /// <summary>
    /// Dashboard API key. When set, the registered <see cref="HttpClient"/> sends HTTP Basic authentication.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// REST API base URL. Defaults to <see cref="OctopusEnergyClient.DefaultBaseUrl"/>.
    /// </summary>
    public Uri BaseAddress { get; set; } = new(OctopusEnergyClient.DefaultBaseUrl);

    /// <summary>
    /// Named <see cref="HttpClient"/> registration. Defaults to <see cref="DefaultHttpClientName"/>.
    /// </summary>
    public string HttpClientName { get; set; } = DefaultHttpClientName;

    /// <summary>
    /// Optional retry policy for HTTP 429 and 503 responses on REST calls.
    /// </summary>
    public OctopusEnergyRetryOptions? RetryOptions { get; set; }
}
