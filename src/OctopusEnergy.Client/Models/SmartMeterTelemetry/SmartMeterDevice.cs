namespace OctopusEnergy.Client.Models.SmartMeterTelemetry;

/// <summary>
/// A SMETS device (ESME or GSME) discovered on the account for Home Mini / CAD telemetry.
/// </summary>
public sealed class SmartMeterDevice
{
    /// <summary>
    /// Gets the EUI64 device id used by <c>smartMeterTelemetry(deviceId:)</c>.
    /// </summary>
    public required string DeviceId { get; init; }

    /// <summary>
    /// Gets the device type (for example <c>ESME</c> or <c>GSME</c>) when returned by the API.
    /// </summary>
    public string? DeviceType { get; init; }

    /// <summary>
    /// Gets the smart device serial number when returned by the API.
    /// </summary>
    public string? SerialNumber { get; init; }

    /// <summary>
    /// Gets the parent meter serial number on the agreement's meter point.
    /// </summary>
    public string? MeterSerialNumber { get; init; }

    /// <summary>
    /// Gets the MPAN or MPRN for the meter point.
    /// </summary>
    public string? SupplyPointIdentifier { get; init; }

    /// <summary>
    /// Gets whether the device was discovered under an electricity or gas agreement.
    /// </summary>
    public EnergyFuel Fuel { get; init; }
}
