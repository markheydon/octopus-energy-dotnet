using System.Text.Json;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests;

/// <summary>
/// Consumption vs UTC Agile price alignment around UK clock changes.
/// </summary>
/// <remarks>
/// <para>
/// <strong>Spring forward:</strong> one local half-hour is skipped. Consumption may switch
/// from <c>Z</c> to <c>+01:00</c> while rates stay UTC. Join on
/// <see cref="DateTimeOffset"/> instants only.
/// </para>
/// <para>
/// <strong>Autumn back:</strong> the local hour from 01:00 to 02:00 occurs twice. The first
/// pass is BST (<c>+01:00</c>); the second is GMT (<c>Z</c>). Matching on wall-clock hour
/// without offset double-counts.
/// </para>
/// <para>
/// <strong>Agile short day:</strong> before 16:00 Europe/London, day-ahead rates may return
/// 46 half-hours instead of 48. That is normal API behaviour, not a matching bug.
/// </para>
/// </remarks>
public sealed class ConsumptionPricePeriodMatchingTests
{
    [Fact]
    public void JoinByInstant_WhenSpringForward_MatchesAllIntervalsWithoutDropping()
    {
        List<ConsumptionInterval> intervals = DeserializeConsumption("consumption-bst-spring-forward.json");
        List<TariffCharge> rates = DeserializeRates("agile-rates-utc-spring-forward.json");

        IReadOnlyList<(ConsumptionInterval Interval, TariffCharge Rate)> matches =
            ConsumptionPricePeriodMatching.JoinByInstant(intervals, rates);

        Assert.Equal(4, matches.Count);
        Assert.All(matches, match => AssertRateContainsIntervalStart(match.Rate, match.Interval.IntervalStart));
    }

    [Fact]
    public void JoinByInstant_WhenAutumnBack_DistinguishesDuplicateLocalHour()
    {
        List<ConsumptionInterval> intervals = DeserializeConsumption("consumption-bst-autumn-back.json");
        List<TariffCharge> rates =
        [
            CreateRate(new DateTimeOffset(2024, 10, 27, 0, 0, 0, TimeSpan.FromHours(1))),
            CreateRate(new DateTimeOffset(2024, 10, 27, 0, 0, 0, TimeSpan.Zero)),
        ];

        IReadOnlyList<(ConsumptionInterval Interval, TariffCharge Rate)> matches =
            ConsumptionPricePeriodMatching.JoinByInstant(intervals, rates);

        Assert.Equal(2, matches.Count);
        Assert.Equal(intervals[1].IntervalStart, matches[0].Interval.IntervalStart);
        Assert.Equal(intervals[2].IntervalStart, matches[1].Interval.IntervalStart);
        Assert.All(matches, match => AssertRateContainsIntervalStart(match.Rate, match.Interval.IntervalStart));
    }

    [Fact]
    public void JoinByWallClockHour_WhenAutumnBack_DoubleCountsDuplicateLocalHour()
    {
        List<ConsumptionInterval> intervals = DeserializeConsumption("consumption-bst-autumn-back.json");
        TariffCharge rate = CreateRate(new DateTimeOffset(2024, 10, 27, 0, 0, 0, TimeSpan.Zero));

        int naiveMatches = intervals.Count(interval =>
            interval.IntervalStart.DateTime.Date == rate.ValidFrom.DateTime.Date
            && interval.IntervalStart.Hour == rate.ValidFrom.Hour);

        Assert.Equal(3, naiveMatches);
    }

    [Fact]
    public void JoinByDateTimeComponent_WhenSpringForward_MatchesDifferentInstants()
    {
        List<ConsumptionInterval> intervals = DeserializeConsumption("consumption-bst-spring-forward.json");
        List<TariffCharge> rates = DeserializeRates("agile-rates-utc-spring-forward.json");

        List<(ConsumptionInterval Interval, TariffCharge Rate)> naiveMatches = intervals
            .SelectMany(interval => rates
                .Where(rate => interval.IntervalStart.DateTime == rate.ValidFrom.DateTime)
                .Select(rate => (interval, rate)))
            .ToList();

        List<(ConsumptionInterval Interval, TariffCharge Rate)> wrongPairs = naiveMatches
            .Where(match => match.Interval.IntervalStart != match.Rate.ValidFrom)
            .ToList();

        Assert.Equal(2, wrongPairs.Count);
        Assert.Contains(wrongPairs, pair => pair.Interval == intervals[2] && pair.Rate == rates[2]);
    }

    [Fact]
    public void AgileShortDayFixture_BeforePublishDeadline_HasFortySixHalfHours()
    {
        PaginatedResponse<TariffCharge> page = Deserialize<PaginatedResponse<TariffCharge>>(
            "agile-rates-short-day-46-slots.json");

        Assert.Equal(46, page.Count);
        Assert.Equal(46, page.Results.Count);
        Assert.NotEqual(48, page.Results.Count);
    }

    [Fact]
    public void FindRateForInterval_WhenInstantMatches_ReturnsRate()
    {
        ConsumptionInterval interval = DeserializeConsumption("consumption-bst-spring-forward.json")[0];
        List<TariffCharge> rates = DeserializeRates("agile-rates-utc-spring-forward.json");

        TariffCharge? rate = ConsumptionPricePeriodMatching.FindRateForInterval(interval, rates);

        Assert.NotNull(rate);
        AssertRateContainsIntervalStart(rate, interval.IntervalStart);
    }

    [Fact]
    public void FindRateForInterval_WhenStartContainedButNotEqualValidFrom_ReturnsRate()
    {
        DateTimeOffset rateStart = new(2024, 3, 31, 0, 0, 0, TimeSpan.Zero);
        TariffCharge rate = CreateRate(rateStart);
        ConsumptionInterval interval = new()
        {
            Consumption = 0.1m,
            IntervalStart = rateStart.AddMinutes(15),
            IntervalEnd = rateStart.AddMinutes(45),
        };

        TariffCharge? match = ConsumptionPricePeriodMatching.FindRateForInterval(interval, [rate]);

        Assert.NotNull(match);
        Assert.Same(rate, match);
        Assert.NotEqual(interval.IntervalStart, match.ValidFrom);
        AssertRateContainsIntervalStart(match, interval.IntervalStart);
    }

    [Fact]
    public void FindRateForInterval_WhenMultipleRatesContainStart_PrefersLatestValidFrom()
    {
        DateTimeOffset instant = new(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        TariffCharge wideRate = new()
        {
            ValueExcVat = 8m,
            ValueIncVat = 8.4m,
            ValidFrom = instant.AddHours(-6),
            ValidTo = instant.AddHours(6),
        };
        TariffCharge narrowRate = CreateRate(instant);

        TariffCharge? match = ConsumptionPricePeriodMatching.FindRateForInterval(
            new ConsumptionInterval
            {
                Consumption = 0.2m,
                IntervalStart = instant,
                IntervalEnd = instant.AddMinutes(30),
            },
            [wideRate, narrowRate]);

        Assert.Same(narrowRate, match);
    }

    private static List<ConsumptionInterval> DeserializeConsumption(string fixtureName)
    {
        PaginatedResponse<ConsumptionInterval> page = Deserialize<PaginatedResponse<ConsumptionInterval>>(fixtureName);
        return page.Results.ToList();
    }

    private static List<TariffCharge> DeserializeRates(string fixtureName)
    {
        PaginatedResponse<TariffCharge> page = Deserialize<PaginatedResponse<TariffCharge>>(fixtureName);
        return page.Results.ToList();
    }

    private static T Deserialize<T>(string fixtureName)
    {
        string json = FixtureFile.Read(fixtureName);
        return JsonSerializer.Deserialize<T>(json, OctopusJsonSerializerOptions.Default)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null.");
    }

    private static TariffCharge CreateRate(DateTimeOffset validFrom)
    {
        return new TariffCharge
        {
            ValueExcVat = 10m,
            ValueIncVat = 10.5m,
            ValidFrom = validFrom,
            ValidTo = validFrom.AddMinutes(30),
        };
    }

    private static void AssertRateContainsIntervalStart(TariffCharge rate, DateTimeOffset intervalStart)
    {
        Assert.True(intervalStart >= rate.ValidFrom);
        Assert.True(rate.ValidTo is null || intervalStart < rate.ValidTo);
    }
}
