namespace OctopusEnergy.Client.Tests;

public sealed class GraphQlRelayLimitsTests
{
    [Fact]
    public void ValidateFirst_WhenAboveMaximum_Throws()
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => GraphQlRelayLimits.ValidateFirst(GraphQlRelayLimits.MaxFirst + 1));

        Assert.Contains("exceeds the documented maximum", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void ValidateFirst_WhenAtMaximum_DoesNotThrow()
    {
        GraphQlRelayLimits.ValidateFirst(GraphQlRelayLimits.MaxFirst);
    }

    [Fact]
    public void ValidateFirst_WhenZero_Throws()
    {
        Assert.Throws<OctopusEnergyRequestException>(() => GraphQlRelayLimits.ValidateFirst(0));
    }

    [Fact]
    public void ValidateFirst_WhenNegative_Throws()
    {
        Assert.Throws<OctopusEnergyRequestException>(() => GraphQlRelayLimits.ValidateFirst(-1));
    }
}
