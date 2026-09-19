using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class RestPageSizeLimitsTests
{
    [Fact]
    public void Validate_WhenPageSizeExceedsRatesMaximum_ThrowsBeforeHttpCall()
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => RestPageSizeLimits.Validate(1_501, RestPageSizeLimits.RatesMaximum));

        Assert.Contains("1501", exception.Message, StringComparison.Ordinal);
        Assert.Contains("1500", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WhenPageSizeWithinMaximum_DoesNotThrow()
    {
        RestPageSizeLimits.Validate(RestPageSizeLimits.RatesMaximum, RestPageSizeLimits.RatesMaximum);
    }

    [Fact]
    public void Validate_WhenPageSizeIsZero_ThrowsBeforeHttpCall()
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => RestPageSizeLimits.Validate(0, RestPageSizeLimits.ConsumptionMaximum));

        Assert.Contains("must be at least 1", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_WhenPageSizeIsNegative_ThrowsBeforeHttpCall()
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => RestPageSizeLimits.Validate(-1, RestPageSizeLimits.ConsumptionMaximum));

        Assert.Contains("must be at least 1", exception.Message, StringComparison.Ordinal);
    }
}
