using OctopusEnergy.Client.Infrastructure.GraphQL;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class KrakenJwtExpiryTests
{
    [Fact]
    public void TryReadExpiryUtc_WhenExpClaimPresent_ReturnsInstant()
    {
        string jwt = "eyJhbGciOiJub25lIn0.eyJleHAiOjQ3MDAwMDAwMDB9.signature";

        DateTimeOffset? expiry = KrakenJwtExpiry.TryReadExpiryUtc(jwt);

        Assert.NotNull(expiry);
        Assert.Equal(4700000000, expiry!.Value.ToUnixTimeSeconds());
    }

    [Fact]
    public void TryReadExpiryUtc_WhenMalformed_ReturnsNull()
    {
        DateTimeOffset? expiry = KrakenJwtExpiry.TryReadExpiryUtc("not-a-jwt");

        Assert.Null(expiry);
    }
}
