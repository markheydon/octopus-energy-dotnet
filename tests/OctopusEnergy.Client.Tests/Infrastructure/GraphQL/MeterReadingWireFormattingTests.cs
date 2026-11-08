using OctopusEnergy.Client.Infrastructure.GraphQL;

namespace OctopusEnergy.Client.Tests.Infrastructure.GraphQL;

public sealed class MeterReadingWireFormattingTests
{
    [Fact]
    public void FormatReadAtDate_WhenUtcInstantInSummer_ConvertsToUkCivilDate()
    {
        DateTimeOffset readAt = new(2024, 6, 30, 23, 30, 0, TimeSpan.Zero);

        string formatted = MeterReadingWireFormatting.FormatReadAtDate(readAt);

        Assert.Equal("2024-07-01", formatted);
    }

    [Fact]
    public void FormatReadAtDate_WhenOffsetAlreadyUkSummer_ReturnsSameCivilDate()
    {
        DateTimeOffset readAt = new(2024, 6, 15, 10, 0, 0, TimeSpan.FromHours(1));

        string formatted = MeterReadingWireFormatting.FormatReadAtDate(readAt);

        Assert.Equal("2024-06-15", formatted);
    }

    [Fact]
    public void FormatReadAtDate_WhenUtcInstantInWinter_UsesUkCivilDate()
    {
        DateTimeOffset readAt = new(2024, 1, 15, 12, 0, 0, TimeSpan.Zero);

        string formatted = MeterReadingWireFormatting.FormatReadAtDate(readAt);

        Assert.Equal("2024-01-15", formatted);
    }
}
