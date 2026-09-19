using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyRetryOptionsTests
{
    [Fact]
    public void Validate_WhenMaxAttemptsIsNegative_ThrowsArgumentOutOfRangeException()
    {
        OctopusEnergyRetryOptions options = new() { MaxAttempts = -1 };

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(options.Validate);

        Assert.Equal("MaxAttempts", exception.ParamName);
    }

    [Fact]
    public void Validate_WhenMaxAttemptsExceedsUpperBound_ThrowsArgumentOutOfRangeException()
    {
        OctopusEnergyRetryOptions options = new()
        {
            MaxAttempts = OctopusEnergyRetryOptions.MaxAttemptsUpperBound + 1,
        };

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(options.Validate);

        Assert.Equal("MaxAttempts", exception.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(OctopusEnergyRetryOptions.MaxAttemptsUpperBound)]
    public void Validate_WhenMaxAttemptsIsWithinBounds_DoesNotThrow(int maxAttempts)
    {
        OctopusEnergyRetryOptions options = new() { MaxAttempts = maxAttempts };

        options.Validate();
    }

    [Fact]
    public void Validate_WhenBaseDelayIsNegative_ThrowsArgumentOutOfRangeException()
    {
        OctopusEnergyRetryOptions options = new() { BaseDelay = TimeSpan.FromMilliseconds(-1) };

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(options.Validate);

        Assert.Equal("BaseDelay", exception.ParamName);
    }

    [Fact]
    public void Validate_WhenMaxDelayIsNegative_ThrowsArgumentOutOfRangeException()
    {
        OctopusEnergyRetryOptions options = new() { MaxDelay = TimeSpan.FromMilliseconds(-1) };

        ArgumentOutOfRangeException exception = Assert.Throws<ArgumentOutOfRangeException>(options.Validate);

        Assert.Equal("MaxDelay", exception.ParamName);
    }
}
