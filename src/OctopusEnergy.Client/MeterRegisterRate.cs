namespace OctopusEnergy.Client;

/// <summary>
/// Register rate type on an electricity meter.
/// </summary>
/// <remarks>
/// Known Octopus wire tokens: <c>STANDARD</c>, <c>DAY</c>, <c>NIGHT</c>.
/// </remarks>
public enum MeterRegisterRate
{
    /// <summary>
    /// Unrecognised or missing wire token.
    /// </summary>
    Unknown,

    /// <summary>
    /// Standard register (<c>STANDARD</c>).
    /// </summary>
    Standard,

    /// <summary>
    /// Day register (<c>DAY</c>).
    /// </summary>
    Day,

    /// <summary>
    /// Night register (<c>NIGHT</c>).
    /// </summary>
    Night,
}
