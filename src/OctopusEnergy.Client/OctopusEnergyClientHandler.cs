using OctopusEnergy.Client.Infrastructure.Http;

namespace OctopusEnergy.Client;

/// <summary>
/// Adds Octopus Energy SDK request headers for use with <c>IHttpClientFactory</c>.
/// </summary>
/// <remarks>
/// Register this handler when configuring a named or typed <see cref="HttpClient"/> so
/// authentication, <c>Accept</c>, and <c>User-Agent</c> are applied per request without
/// mutating shared <see cref="HttpClient.DefaultRequestHeaders"/>.
/// When both this handler and <see cref="OctopusEnergyClient"/> are configured with an API key,
/// the client applies authentication on the request first; this handler does not replace an
/// existing <c>Authorization</c> header. Prefer configuring the key in one place only.
/// See <see href="https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/how-to/authentication.md">authentication</see>
/// for a hosted registration example.
/// </remarks>
public sealed class OctopusEnergyClientHandler : DelegatingHandler
{
    private readonly string? _apiKey;

    /// <summary>
    /// Creates a handler for unauthenticated public catalogue calls.
    /// </summary>
    public OctopusEnergyClientHandler()
    {
    }

    /// <summary>
    /// Creates a handler that sends the API key as HTTP Basic authentication.
    /// </summary>
    /// <param name="apiKey">
    /// Dashboard API key. Sent as HTTP Basic authentication with an empty password.
    /// Treat as a secret; the SDK never logs it.
    /// </param>
    public OctopusEnergyClientHandler(string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        _apiKey = apiKey;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        OctopusEnergyRequestHeaders.Apply(request, _apiKey);
        return base.SendAsync(request, cancellationToken);
    }
}
