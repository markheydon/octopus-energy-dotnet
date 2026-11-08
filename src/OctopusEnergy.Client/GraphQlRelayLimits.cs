namespace OctopusEnergy.Client;

/// <summary>
/// Documented GraphQL Relay and operational limits from the Octopus Kraken API.
/// </summary>
public static class GraphQlRelayLimits
{
    /// <summary>
    /// Maximum <c>first</c> argument on Relay connections.
    /// </summary>
    public const int MaxFirst = 100;

    /// <summary>
    /// Maximum query complexity per request (<c>KT-CT-1188</c> when exceeded).
    /// </summary>
    public const int MaxComplexityPerRequest = 200;

    /// <summary>
    /// Default hourly points allowance for dashboard account users (BLOCK mode when exceeded).
    /// </summary>
    public const int DefaultHourlyPointsAllowance = 50_000;

    /// <summary>
    /// Maximum nodes per request (<c>KT-CT-1189</c> when exceeded).
    /// </summary>
    public const int MaxNodesPerRequest = 10_000;

    /// <summary>
    /// Validates that <paramref name="first"/> is within documented Relay limits.
    /// </summary>
    /// <param name="first">The requested page size (<c>first</c>).</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="first"/> is less than 1 or exceeds <see cref="MaxFirst"/>.
    /// </exception>
    public static void ValidateFirst(int first)
    {
        if (first < 1)
        {
            throw new OctopusEnergyRequestException(
                $"first {first} must be at least 1.");
        }

        if (first > MaxFirst)
        {
            throw new OctopusEnergyRequestException(
                $"first {first} exceeds the documented maximum of {MaxFirst}.");
        }
    }
}
