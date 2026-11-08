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
    /// Requires the host to reference <c>Microsoft.Extensions.Http</c>. When <see cref="OctopusEnergyClientOptions.ApiKey"/>
    /// is set, authentication is applied through <see cref="OctopusEnergyClientHandler"/> on the named client.
    /// Do not configure a different API key on both the handler and <see cref="OctopusEnergyClient"/> constructor.
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

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            string apiKey = options.ApiKey.Trim();
            builder.AddHttpMessageHandler(() => new OctopusEnergyClientHandler(apiKey));
        }
        else
        {
            builder.AddHttpMessageHandler(() => new OctopusEnergyClientHandler());
        }

        services.AddSingleton<IOctopusEnergyClient>(sp =>
        {
            IHttpClientFactory factory = sp.GetRequiredService<IHttpClientFactory>();
            HttpClient httpClient = factory.CreateClient(httpClientName);
            return new OctopusEnergyClient(httpClient, options.RetryOptions);
        });

        return services;
    }
}
