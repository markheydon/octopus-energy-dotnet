using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// An electricity meter at a meter point.
/// </summary>
public sealed class ElectricityMeter
{
    private IReadOnlyList<MeterRegister> _registers = [];

    /// <summary>
    /// Meter serial number.
    /// </summary>
    [JsonPropertyName("serial_number")]
    public string SerialNumber { get; init; } = string.Empty;

    /// <summary>
    /// Registers on the meter.
    /// </summary>
    [JsonPropertyName("registers")]
    public IReadOnlyList<MeterRegister> Registers
    {
        get => _registers;
        init => _registers = value ?? [];
    }
}
