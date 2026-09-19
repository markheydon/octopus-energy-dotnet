using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Consumption;

/// <summary>
/// A consumption interval for electricity or gas.
/// </summary>
/// <remarks>
/// Export MPANs still use the <see cref="Consumption"/> field name. Electricity is
/// reported in kWh (0.001 precision). Gas is SMETS1 kWh or SMETS2 m³ depending on
/// the meter; see <see cref="ConsumptionUnits"/> when present.
/// </remarks>
public sealed class ConsumptionInterval
{
    /// <summary>
    /// Consumption for the interval (kWh for electricity; kWh or m³ for gas).
    /// </summary>
    [JsonPropertyName("consumption")]
    public decimal Consumption { get; init; }

    /// <summary>
    /// Interval start (ISO 8601 with <c>Z</c> or offset).
    /// </summary>
    [JsonPropertyName("interval_start")]
    public DateTimeOffset IntervalStart { get; init; }

    /// <summary>
    /// Interval end (ISO 8601 with <c>Z</c> or offset).
    /// </summary>
    [JsonPropertyName("interval_end")]
    public DateTimeOffset IntervalEnd { get; init; }

    /// <summary>
    /// Unit when returned by the API (for example <c>kWh</c> or <c>m3</c>).
    /// </summary>
    [JsonPropertyName("consumption_units")]
    public string? ConsumptionUnits { get; init; }

    /// <summary>
    /// Parsed gas unit when <see cref="ConsumptionUnits"/> is present.
    /// </summary>
    /// <remarks>
    /// Applies to gas intervals only. Electricity consumption is always reported in kWh;
    /// electricity callers should ignore this property.
    /// </remarks>
    public GasConsumptionUnit GasUnit => ParseGasUnit(ConsumptionUnits);

    private static GasConsumptionUnit ParseGasUnit(string? units)
    {
        if (string.IsNullOrWhiteSpace(units))
        {
            return GasConsumptionUnit.Unknown;
        }

        return units.Trim() switch
        {
            "kWh" => GasConsumptionUnit.KilowattHours,
            "m3" or "m³" => GasConsumptionUnit.CubicMetres,
            _ => GasConsumptionUnit.Unknown,
        };
    }
}
