using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// A gas meter point (MPRN) on a property.
/// </summary>
public sealed class GasMeterPoint
{
    private IReadOnlyList<GasMeter> _meters = [];
    private IReadOnlyList<TariffAgreement> _agreements = [];

    /// <summary>
    /// Meter point reference number (MPRN).
    /// </summary>
    [JsonPropertyName("mprn")]
    public string Mprn { get; init; } = string.Empty;

    /// <summary>
    /// Standard annual consumption, or <see langword="null"/> if not returned.
    /// </summary>
    [JsonPropertyName("consumption_standard")]
    public int? ConsumptionStandard { get; init; }

    /// <summary>
    /// Meters at this point, including old and new serial numbers.
    /// </summary>
    [JsonPropertyName("meters")]
    public IReadOnlyList<GasMeter> Meters
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
}
