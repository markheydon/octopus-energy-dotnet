using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.RateLimits;

namespace OctopusEnergy.Client.Services.RateLimits;

/// <summary>
/// Kraken operational rate-limit information for the authenticated viewer.
/// </summary>
public sealed class RateLimitService : IRateLimitService
{
    private readonly GraphQlClient? _graphQl;

    internal RateLimitService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<RateLimitInfo> GetRateLimitInfoAsync(CancellationToken cancellationToken = default)
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query rate limit information.");
        }

        RateLimitInfoQueryData data = await _graphQl.ExecuteAsync<RateLimitInfoQueryData>(
            RateLimitInfoQueries.GetRateLimitInfo,
            cancellationToken).ConfigureAwait(false);

        return data.RateLimitInfo
            ?? throw new OctopusEnergyParseException("The rateLimitInfo query returned no data.");
    }
}
