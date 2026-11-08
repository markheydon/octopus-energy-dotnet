namespace OctopusEnergy.Client;

/// <summary>
/// REST charge list endpoints under a product tariff.
/// </summary>
public enum TariffChargeKind
{
    /// <summary>
    /// Standing charges (<c>standing-charges/</c>).
    /// </summary>
    StandingCharges,

    /// <summary>
    /// Standard unit rates (<c>standard-unit-rates/</c>).
    /// </summary>
    StandardUnitRates,

    /// <summary>
    /// Day unit rates for dual-register electricity (<c>day-unit-rates/</c>).
    /// </summary>
    DayUnitRates,

    /// <summary>
    /// Night unit rates for dual-register electricity (<c>night-unit-rates/</c>).
    /// </summary>
    NightUnitRates,

    /// <summary>
    /// EV device off-peak unit rates for electricity tariffs (<c>ev-device-off-peak-unit-rates/</c>).
    /// </summary>
    EvDeviceOffPeakUnitRates,

    /// <summary>
    /// EV device peak unit rates for electricity tariffs (<c>ev-device-peak-unit-rates/</c>).
    /// </summary>
    EvDevicePeakUnitRates,
}
