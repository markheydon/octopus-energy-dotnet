using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// An electricity meter point (MPAN) on a property.
/// </summary>
public sealed class ElectricityMeterPoint
{
    private IReadOnlyList<ElectricityMeter> _meters = [];
    private IReadOnlyList<TariffAgreement> _agreements = [];

    /// <summary>
    /// Meter point administration number (MPAN).
    /// </summary>
    [JsonPropertyName("mpan")]
    public string Mpan { get; init; } = string.Empty;

    /// <summary>
    /// Profile class.
    /// </summary>
    [JsonPropertyName("profile_class")]
    public int ProfileClass { get; init; }

    /// <summary>
    /// Standard annual consumption in kWh, or <see langword="null"/> if not returned.
    /// </summary>
    [JsonPropertyName("consumption_standard")]
    public int? ConsumptionStandard { get; init; }

    /// <summary>
    /// Meters at this point, including old and new serial numbers.
    /// </summary>
    [JsonPropertyName("meters")]
    public IReadOnlyList<ElectricityMeter> Meters
    {
        get => _meters;
        init => _meters = value ?? [];
    }

    /// <summary>
    /// Tariff agreements for this meter point.
    /// </summary>
    [JsonPropertyName("agreements")]
    public IReadOnlyList<TariffAgreement> Agreements
    {
        get => _agreements;
        init => _agreements = value ?? [];
    }

    /// <summary>
    /// Whether this MPAN is an export meter. <see langword="null"/> when the API omits the field.
    /// </summary>
    [JsonPropertyName("is_export")]
    public bool? IsExport { get; init; }
}
