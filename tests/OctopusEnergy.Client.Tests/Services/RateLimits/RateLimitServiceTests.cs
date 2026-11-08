using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.RateLimits;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.RateLimits;

public sealed class RateLimitServiceTests
{
    [Fact]
    public async Task GetRateLimitInfoAsync_WhenAuthenticated_ReturnsPointsAllowance()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-rate-limit-info.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        RateLimitInfo info = await client.RateLimits.GetRateLimitInfoAsync(CancellationToken.None);

        Assert.NotNull(info.PointsAllowanceRateLimit);
        Assert.Equal(50_000, info.PointsAllowanceRateLimit!.Limit);
        Assert.Equal(49_872, info.PointsAllowanceRateLimit.Remaining);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetRateLimitInfoAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.RateLimits.GetRateLimitInfoAsync(CancellationToken.None));
    }
}
