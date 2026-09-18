using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyTimeTests
{
    [Fact]
    public void AssumeEuropeLondon_WhenWinter_GivesZeroOffset()
    {
        DateTimeOffset result = OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 1, 15, 12, 0, 0));

        Assert.Equal(TimeSpan.Zero, result.Offset);
        Assert.Equal(new DateTimeOffset(2024, 1, 15, 12, 0, 0, TimeSpan.Zero), result);
    }

    [Fact]
    public void AssumeEuropeLondon_WhenSummer_GivesOneHourOffset()
    {
        DateTimeOffset result = OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 7, 15, 12, 0, 0));

        Assert.Equal(TimeSpan.FromHours(1), result.Offset);
        Assert.Equal(new DateTimeOffset(2024, 7, 15, 12, 0, 0, TimeSpan.FromHours(1)), result);
    }

    [Fact]
    public void AssumeEuropeLondon_WhenUtcKind_Throws()
    {
        Assert.Throws<OctopusEnergyRequestException>(
            () => OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 1, 15, 12, 0, 0, DateTimeKind.Utc)));
    }
}
