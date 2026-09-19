using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client;

/// <summary>
/// Helpers for aligning consumption intervals with UTC Agile unit-rate periods.
/// </summary>
/// <remarks>
/// <para>
/// Price endpoints return exact requested periods in UTC. Consumption intervals may
/// use <c>Z</c> or <c>+01:00</c> around BST and can overlap the requested window.
/// Always join on <see cref="DateTimeOffset"/> instants, not wall-clock hour or
/// <see cref="DateTime"/> without offset.
/// </para>
/// <para>
/// Matching finds the rate period <c>[ValidFrom, ValidTo)</c> that contains the
/// interval start instant. It does not match on full interval overlap: when the
/// interval body crosses a rate boundary but the start falls outside every fetched
/// rate window, no match is returned. This is a join helper, not a cost calculator:
/// it does not apply half-to-even billing rounding or split an interval across two rates.
/// </para>
/// </remarks>
public static class ConsumptionPricePeriodMatching
{
    /// <summary>
    /// Finds the rate whose validity period contains the interval start instant.
    /// </summary>
    /// <param name="interval">Consumption interval.</param>
    /// <param name="rates">Unit rates for the same period.</param>
    /// <returns>
    /// The rate whose half-open period <c>[ValidFrom, ValidTo)</c> contains
    /// <see cref="ConsumptionInterval.IntervalStart"/>, or <see langword="null"/> when
    /// none applies. When multiple rates contain the start, the one with the latest
    /// <see cref="TariffCharge.ValidFrom"/> wins; when <see cref="TariffCharge.ValidFrom"/>
    /// ties, the first rate in <paramref name="rates"/> wins.
    /// </returns>
    public static TariffCharge? FindRateForInterval(
        ConsumptionInterval interval,
        IReadOnlyList<TariffCharge> rates)
    {
        ArgumentNullException.ThrowIfNull(interval);
        ArgumentNullException.ThrowIfNull(rates);

        return FindContainingRate(interval.IntervalStart, rates);
    }

    /// <summary>
    /// Joins consumption intervals to rates by the interval-start instant within a rate period.
    /// </summary>
    /// <param name="intervals">Consumption intervals.</param>
    /// <param name="rates">Unit rates covering the same period.</param>
    /// <returns>
    /// Pairs where the rate validity period contains
    /// <see cref="ConsumptionInterval.IntervalStart"/>. Intervals with no matching rate
    /// are omitted.
    /// </returns>
    public static IReadOnlyList<(ConsumptionInterval Interval, TariffCharge Rate)> JoinByInstant(
        IReadOnlyList<ConsumptionInterval> intervals,
        IReadOnlyList<TariffCharge> rates)
    {
        ArgumentNullException.ThrowIfNull(intervals);
        ArgumentNullException.ThrowIfNull(rates);

        List<(ConsumptionInterval, TariffCharge)> matches = new();

        foreach (ConsumptionInterval interval in intervals)
        {
            TariffCharge? rate = FindContainingRate(interval.IntervalStart, rates);
            if (rate is not null)
            {
                matches.Add((interval, rate));
            }
        }

        return matches;
    }

    private static TariffCharge? FindContainingRate(DateTimeOffset instant, IReadOnlyList<TariffCharge> rates)
    {
        TariffCharge? best = null;

        foreach (TariffCharge rate in rates)
        {
            if (!ContainsInstant(rate, instant))
            {
                continue;
            }

            if (best is null || rate.ValidFrom > best.ValidFrom)
            {
                best = rate;
            }
        }

        return best;
    }

    private static bool ContainsInstant(TariffCharge rate, DateTimeOffset instant)
    {
        if (instant < rate.ValidFrom)
        {
            return false;
        }

        return rate.ValidTo is null || instant < rate.ValidTo;
    }
}
