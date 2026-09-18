using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client;

/// <summary>
/// Helpers for aligning consumption intervals with UTC Agile unit-rate periods.
/// </summary>
/// <remarks>
/// Price endpoints return exact requested periods in UTC. Consumption intervals may
/// use <c>Z</c> or <c>+01:00</c> around BST. Always join on
/// <see cref="DateTimeOffset"/> instants, not wall-clock hour or
/// <see cref="DateTime"/> without offset.
/// </remarks>
public static class ConsumptionPricePeriodMatching
{
    /// <summary>
    /// Finds the rate whose <see cref="TariffCharge.ValidFrom"/> equals the interval start.
    /// </summary>
    /// <param name="interval">Consumption interval.</param>
    /// <param name="rates">Unit rates for the same period.</param>
    /// <returns>The matching rate, or <see langword="null"/> when none aligns.</returns>
    public static TariffCharge? FindRateForInterval(
        ConsumptionInterval interval,
        IReadOnlyList<TariffCharge> rates)
    {
        ArgumentNullException.ThrowIfNull(interval);
        ArgumentNullException.ThrowIfNull(rates);

        foreach (TariffCharge rate in rates)
        {
            if (rate.ValidFrom == interval.IntervalStart)
            {
                return rate;
            }
        }

        return null;
    }

    /// <summary>
    /// Joins consumption intervals to rates by exact <see cref="DateTimeOffset"/> instant.
    /// </summary>
    /// <param name="intervals">Consumption intervals.</param>
    /// <param name="rates">Unit rates covering the same instants.</param>
    /// <returns>Pairs where <see cref="TariffCharge.ValidFrom"/> equals <see cref="ConsumptionInterval.IntervalStart"/>.</returns>
    public static IReadOnlyList<(ConsumptionInterval Interval, TariffCharge Rate)> JoinByInstant(
        IReadOnlyList<ConsumptionInterval> intervals,
        IReadOnlyList<TariffCharge> rates)
    {
        ArgumentNullException.ThrowIfNull(intervals);
        ArgumentNullException.ThrowIfNull(rates);

        Dictionary<DateTimeOffset, TariffCharge> ratesByStart = new();

        foreach (TariffCharge rate in rates)
        {
            ratesByStart[rate.ValidFrom] = rate;
        }

        List<(ConsumptionInterval, TariffCharge)> matches = new();

        foreach (ConsumptionInterval interval in intervals)
        {
            if (ratesByStart.TryGetValue(interval.IntervalStart, out TariffCharge? rate))
            {
                matches.Add((interval, rate));
            }
        }

        return matches;
    }
}
