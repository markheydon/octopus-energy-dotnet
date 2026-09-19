namespace OctopusEnergy.Client;

/// <summary>
/// Timezone conventions for Octopus Energy REST datetimes.
/// </summary>
/// <remarks>
/// Octopus treats datetimes without an offset as <strong>Europe/London</strong>.
/// Consumption intervals may switch between <c>Z</c> and <c>+01:00</c> around BST;
/// Agile unit rates stay UTC. Always use <see cref="DateTimeOffset"/> in models and
/// send <c>Z</c> or an explicit offset from the SDK.
/// </remarks>
public static class OctopusEnergyTime
{
    /// <summary>
    /// IANA timezone id used when the API omits an offset.
    /// </summary>
    public const string EuropeLondonTimeZoneId = "Europe/London";

    /// <summary>
    /// UK local time (<see cref="EuropeLondonTimeZoneId"/>), including BST transitions.
    /// </summary>
    public static TimeZoneInfo EuropeLondon { get; } = ResolveEuropeLondon();

    /// <summary>
    /// Interprets a civil datetime in Europe/London when the API omits an offset.
    /// </summary>
    /// <param name="localDateTime">Wall-clock time in the UK.</param>
    /// <returns>The instant with the correct offset for that civil time.</returns>
    /// <remarks>
    /// Ambiguous local times during the autumn clock change (the repeated hour) resolve to
    /// standard time (GMT) — the later instant. Civil times in the spring-forward gap do not
    /// exist and throw <see cref="OctopusEnergyRequestException"/>.
    /// </remarks>
    public static DateTimeOffset AssumeEuropeLondon(DateTime localDateTime)
    {
        if (localDateTime.Kind == DateTimeKind.Utc)
        {
            throw new OctopusEnergyRequestException(
                "Cannot assume Europe/London for a UTC DateTime. Use DateTimeKind.Unspecified for civil UK times.");
        }

        DateTime unspecified = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);

        if (EuropeLondon.IsInvalidTime(unspecified))
        {
            throw new OctopusEnergyRequestException(
                "The specified civil time does not exist in Europe/London (clock spring-forward gap).");
        }

        DateTime utc = TimeZoneInfo.ConvertTimeToUtc(unspecified, EuropeLondon);
        TimeSpan offset = EuropeLondon.GetUtcOffset(utc);
        return new DateTimeOffset(unspecified, offset);
    }

    private static TimeZoneInfo ResolveEuropeLondon()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(EuropeLondonTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        }
    }
}
