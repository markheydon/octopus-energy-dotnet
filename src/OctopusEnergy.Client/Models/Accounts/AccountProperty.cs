using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// A property on a customer account.
/// </summary>
public sealed class AccountProperty
{
    private IReadOnlyList<ElectricityMeterPoint> _electricityMeterPoints = [];
    private IReadOnlyList<GasMeterPoint> _gasMeterPoints = [];

    /// <summary>
    /// Property identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; init; }

    /// <summary>
    /// When the customer moved in, or <see langword="null"/> if unknown.
    /// </summary>
    [JsonPropertyName("moved_in_at")]
    public DateTimeOffset? MovedInAt { get; init; }

    /// <summary>
    /// When the customer moved out, or <see langword="null"/> if still at the property.
    /// </summary>
    [JsonPropertyName("moved_out_at")]
    public DateTimeOffset? MovedOutAt { get; init; }

    /// <summary>
    /// First address line.
    /// </summary>
    [JsonPropertyName("address_line_1")]
    public string AddressLine1 { get; init; } = string.Empty;

    /// <summary>
    /// Second address line.
    /// </summary>
    [JsonPropertyName("address_line_2")]
    public string? AddressLine2 { get; init; }

    /// <summary>
    /// Third address line.
    /// </summary>
    [JsonPropertyName("address_line_3")]
    public string? AddressLine3 { get; init; }

    /// <summary>
    /// Town or city.
    /// </summary>
    [JsonPropertyName("town")]
    public string Town { get; init; } = string.Empty;

    /// <summary>
    /// County.
    /// </summary>
    [JsonPropertyName("county")]
    public string? County { get; init; }

    /// <summary>
    /// Postcode.
    /// </summary>
    [JsonPropertyName("postcode")]
    public string Postcode { get; init; } = string.Empty;

    /// <summary>
    /// Electricity meter points at the property.
    /// </summary>
    [JsonPropertyName("electricity_meter_points")]
    public IReadOnlyList<ElectricityMeterPoint> ElectricityMeterPoints
    {
        get => _electricityMeterPoints;
        init => _electricityMeterPoints = value ?? [];
    }

    /// <summary>
    /// Gas meter points at the property.
    /// </summary>
    [JsonPropertyName("gas_meter_points")]
    public IReadOnlyList<GasMeterPoint> GasMeterPoints
    {
        get => _gasMeterPoints;
        init => _gasMeterPoints = value ?? [];
    }
}
