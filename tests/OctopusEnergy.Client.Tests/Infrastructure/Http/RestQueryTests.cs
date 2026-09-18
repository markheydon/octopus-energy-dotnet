using OctopusEnergy.Client.Infrastructure.Http;

namespace OctopusEnergy.Client.Tests.Infrastructure.Http;

public sealed class RestQueryTests
{
    [Fact]
    public void FormatDateTimeOffset_WhenUtc_SerialisesWithZ()
    {
        DateTimeOffset value = new(2024, 3, 31, 0, 0, 0, TimeSpan.Zero);

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2024-03-31T00:00:00Z", formatted);
    }

    [Fact]
    public void FormatDateTimeOffset_WhenLondonOffset_NormalisesToUtcZ()
    {
        DateTimeOffset value = new(2024, 7, 15, 12, 0, 0, TimeSpan.FromHours(1));

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2024-07-15T11:00:00Z", formatted);
    }

    [Fact]
    public void FormatDateTimeOffset_WhenFractionalSeconds_TrimsTrailingZeros()
    {
        DateTimeOffset value = new DateTimeOffset(2023, 11, 10, 0, 21, 44, 970, TimeSpan.Zero).AddTicks(7110);

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2023-11-10T00:21:44.970711Z", formatted);
    }
}
