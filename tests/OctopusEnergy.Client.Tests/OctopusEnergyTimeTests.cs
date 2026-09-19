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

    [Fact]
    public void AssumeEuropeLondon_WhenSpringForwardGap_Throws()
    {
        Assert.Throws<OctopusEnergyRequestException>(
            () => OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 3, 31, 1, 30, 0)));
    }

    [Fact]
    public void AssumeEuropeLondon_WhenAutumnBackAmbiguous_ResolvesToStandardTime()
    {
        DateTimeOffset result = OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 10, 27, 1, 30, 0));

        Assert.Equal(TimeSpan.Zero, result.Offset);
        Assert.Equal(new DateTimeOffset(2024, 10, 27, 1, 30, 0, TimeSpan.Zero), result);
        Assert.Equal(new DateTimeOffset(2024, 10, 27, 1, 30, 0, TimeSpan.Zero).ToUniversalTime(), result.ToUniversalTime());
    }

    [Fact]
    public void AssumeEuropeLondon_WhenAutumnBackAmbiguous_DiffersFromDaylightSavingInstant()
    {
        DateTimeOffset daylightSaving = new(2024, 10, 27, 1, 30, 0, TimeSpan.FromHours(1));
        DateTimeOffset standardTime = OctopusEnergyTime.AssumeEuropeLondon(new DateTime(2024, 10, 27, 1, 30, 0));

        Assert.NotEqual(daylightSaving, standardTime);
        Assert.True(standardTime > daylightSaving);
    }
}
