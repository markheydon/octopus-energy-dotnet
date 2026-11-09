using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;

namespace OctopusEnergy.Client.DependencyInjection;

/// <summary>
/// Registers <see cref="IOctopusEnergyClient"/> with <see cref="IHttpClientFactory"/>.
/// </summary>
public static class OctopusEnergyClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds a named <see cref="HttpClient"/> and a singleton <see cref="IOctopusEnergyClient"/>.
    /// </summary>
    /// <param name="services">The application service collection.</param>
    /// <param name="configure">Optional configuration for the API key, base URL, and retry policy.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    /// <remarks>
    /// <c>Microsoft.Extensions.Http</c> is referenced transitively by <c>OctopusEnergy.Client</c>.
    /// When <see cref="OctopusEnergyClientOptions.ApiKey"/> is set, the same key wires REST authentication on the
    /// named <see cref="HttpClient"/> and initialises GraphQL-backed services on <see cref="OctopusEnergyClient"/>.
    /// Do not supply a different API key on another handler or <see cref="OctopusEnergyClient"/> constructor for the same client.
    /// </remarks>
    public static IServiceCollection AddOctopusEnergyClient(
        this IServiceCollection services,
        Action<OctopusEnergyClientOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        OctopusEnergyClientOptions options = new();
        configure?.Invoke(options);

        string httpClientName = string.IsNullOrWhiteSpace(options.HttpClientName)
            ? OctopusEnergyClientOptions.DefaultHttpClientName
            : options.HttpClientName.Trim();

        IHttpClientBuilder builder = services.AddHttpClient(httpClientName, client =>
        {
            client.BaseAddress = options.BaseAddress;
        });

        string? apiKeyForClient = string.IsNullOrWhiteSpace(options.ApiKey)
            ? null
            : options.ApiKey.Trim();

        if (apiKeyForClient is null)
        {
            builder.AddHttpMessageHandler(() => new OctopusEnergyClientHandler());
        }
        else
        {
            builder.AddHttpMessageHandler(() => new OctopusEnergyClientHandler(apiKeyForClient));
        }

        services.AddSingleton<IOctopusEnergyClient>(sp =>
        {
            IHttpClientFactory factory = sp.GetRequiredService<IHttpClientFactory>();
            HttpClient httpClient = factory.CreateClient(httpClientName);
            return apiKeyForClient is null
                ? new OctopusEnergyClient(httpClient, options.RetryOptions)
                : new OctopusEnergyClient(apiKeyForClient, httpClient, options.RetryOptions);
        });

        return services;
    }
}
