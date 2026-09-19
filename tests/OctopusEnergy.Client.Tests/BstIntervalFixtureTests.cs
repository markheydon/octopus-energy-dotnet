using System.Text.Json;
using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests;

/// <summary>
/// BST transition fixtures for consumption (<c>Z</c> vs <c>+01:00</c>) and UTC Agile rates.
/// </summary>
public sealed class BstIntervalFixtureTests
{
    [Fact]
    public void Deserialize_SpringForwardConsumption_PreservesMixedOffsets()
    {
        ConsumptionPage page = Deserialize<ConsumptionPage>("consumption-bst-spring-forward.json");

        Assert.Equal(4, page.Results.Count);

        ConsumptionIntervalWire first = page.Results[0];
        Assert.Equal(TimeSpan.Zero, first.IntervalStart.Offset);
        Assert.Equal(new DateTimeOffset(2024, 3, 31, 0, 0, 0, TimeSpan.Zero), first.IntervalStart);

        ConsumptionIntervalWire third = page.Results[2];
        Assert.Equal(TimeSpan.FromHours(1), third.IntervalStart.Offset);
        Assert.Equal(new DateTimeOffset(2024, 3, 31, 1, 0, 0, TimeSpan.FromHours(1)), third.IntervalStart);
        Assert.Equal(first.IntervalStart, third.IntervalStart.ToUniversalTime());
    }

    [Fact]
    public void Deserialize_AutumnBackConsumption_PreservesDuplicateLocalHour()
    {
        ConsumptionPage page = Deserialize<ConsumptionPage>("consumption-bst-autumn-back.json");

        Assert.Equal(4, page.Results.Count);

        ConsumptionIntervalWire bstHour = page.Results[1];
        ConsumptionIntervalWire gmtHour = page.Results[2];

        Assert.Equal(TimeSpan.FromHours(1), bstHour.IntervalStart.Offset);
        Assert.Equal(TimeSpan.Zero, gmtHour.IntervalStart.Offset);
        Assert.Equal(bstHour.IntervalStart.Hour, gmtHour.IntervalStart.Hour);
        Assert.NotEqual(bstHour.IntervalStart, gmtHour.IntervalStart);
        Assert.True(gmtHour.IntervalStart > bstHour.IntervalStart);
    }

    [Fact]
    public void Deserialize_AgileRatesSpringForward_StayUtc()
    {
        AgileRatesPage page = Deserialize<AgileRatesPage>("agile-rates-utc-spring-forward.json");

        Assert.All(page.Results, rate => Assert.Equal(TimeSpan.Zero, rate.ValidFrom.Offset));
        Assert.Equal(new DateTimeOffset(2024, 3, 31, 0, 30, 0, TimeSpan.Zero), page.Results[1].ValidFrom);
    }

    [Fact]
    public void MatchByWallClockHour_WhenAutumnBack_DoubleCountsDuplicateLocalHour()
    {
        ConsumptionPage consumption = Deserialize<ConsumptionPage>("consumption-bst-autumn-back.json");
        AgileRatesPage rates = new()
        {
            Results =
            [
                new AgileRateWire
                {
                    ValidFrom = new DateTimeOffset(2024, 10, 27, 0, 0, 0, TimeSpan.Zero),
                    ValidTo = new DateTimeOffset(2024, 10, 27, 0, 30, 0, TimeSpan.Zero),
                },
            ],
        };

        List<(ConsumptionIntervalWire Interval, AgileRateWire Rate)> naiveMatches = consumption.Results
            .SelectMany(interval => rates.Results
                .Where(rate =>
                    interval.IntervalStart.DateTime.Date == rate.ValidFrom.DateTime.Date
                    && interval.IntervalStart.Hour == rate.ValidFrom.Hour)
                .Select(rate => (interval, rate)))
            .ToList();

        Assert.Equal(3, naiveMatches.Count);
        Assert.Contains(consumption.Results[1], naiveMatches.Select(match => match.Interval));
        Assert.Contains(consumption.Results[2], naiveMatches.Select(match => match.Interval));
        Assert.Contains(consumption.Results[3], naiveMatches.Select(match => match.Interval));
        Assert.NotEqual(consumption.Results[1].IntervalStart, consumption.Results[2].IntervalStart);
    }

    [Fact]
    public void MatchByIntervalStartInRatePeriod_WhenSpringForward_KeepsAllIntervals()
    {
        ConsumptionPage consumption = Deserialize<ConsumptionPage>("consumption-bst-spring-forward.json");
        AgileRatesPage rates = Deserialize<AgileRatesPage>("agile-rates-utc-spring-forward.json");

        List<ConsumptionIntervalWire> correctMatches = consumption.Results
            .Where(interval => rates.Results.Any(rate => RatePeriodContainsInstant(rate, interval.IntervalStart)))
            .ToList();

        Assert.Equal(4, correctMatches.Count);
    }

    private static bool RatePeriodContainsInstant(AgileRateWire rate, DateTimeOffset instant)
    {
        if (instant < rate.ValidFrom)
        {
            return false;
        }

        return instant < rate.ValidTo;
    }

    private static T Deserialize<T>(string fixtureName)
    {
        string json = FixtureFile.Read(fixtureName);
        return JsonSerializer.Deserialize<T>(json, OctopusJsonSerializerOptions.Default)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null.");
    }

    private sealed class ConsumptionPage
    {
        [JsonPropertyName("results")]
        public List<ConsumptionIntervalWire> Results { get; init; } = [];
    }

    private sealed class ConsumptionIntervalWire
    {
        [JsonPropertyName("consumption")]
        public decimal Consumption { get; init; }

        [JsonPropertyName("interval_start")]
        public DateTimeOffset IntervalStart { get; init; }

        [JsonPropertyName("interval_end")]
        public DateTimeOffset IntervalEnd { get; init; }
    }

    private sealed class AgileRatesPage
    {
        [JsonPropertyName("results")]
        public List<AgileRateWire> Results { get; init; } = [];
    }

    private sealed class AgileRateWire
    {
        [JsonPropertyName("value_exc_vat")]
        public decimal ValueExcVat { get; init; }

        [JsonPropertyName("value_inc_vat")]
        public decimal ValueIncVat { get; init; }

        [JsonPropertyName("valid_from")]
        public DateTimeOffset ValidFrom { get; init; }

        [JsonPropertyName("valid_to")]
        public DateTimeOffset ValidTo { get; init; }
    }
}
