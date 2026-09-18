using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Sample consumption figures used for product quotes.
/// </summary>
public sealed class ProductSampleConsumptionProfile
{
    /// <summary>
    /// Standard electricity consumption in kWh.
    /// </summary>
    [JsonPropertyName("electricity_standard")]
    public decimal? ElectricityStandard { get; init; }

    /// <summary>
    /// Day-rate electricity consumption in kWh.
    /// </summary>
    [JsonPropertyName("electricity_day")]
    public decimal? ElectricityDay { get; init; }

    /// <summary>
    /// Night-rate electricity consumption in kWh.
    /// </summary>
    [JsonPropertyName("electricity_night")]
    public decimal? ElectricityNight { get; init; }

    /// <summary>
    /// Standard gas consumption in kWh.
    /// </summary>
    [JsonPropertyName("gas_standard")]
    public decimal? GasStandard { get; init; }
}

/// <summary>
/// Sample consumption figures grouped by tariff register type.
/// </summary>
public sealed class ProductSampleConsumption
{
    /// <summary>
    /// Single-rate electricity sample consumption.
    /// </summary>
    [JsonPropertyName("electricity_single_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleConsumptionProfile>))]
    public ProductSampleConsumptionProfile? ElectricitySingleRate { get; init; }

    /// <summary>
    /// Dual-rate electricity sample consumption.
    /// </summary>
    [JsonPropertyName("electricity_dual_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleConsumptionProfile>))]
    public ProductSampleConsumptionProfile? ElectricityDualRate { get; init; }

    /// <summary>
    /// Dual-fuel single-rate sample consumption.
    /// </summary>
    [JsonPropertyName("dual_fuel_single_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleConsumptionProfile>))]
    public ProductSampleConsumptionProfile? DualFuelSingleRate { get; init; }

    /// <summary>
    /// Dual-fuel dual-rate sample consumption.
    /// </summary>
    [JsonPropertyName("dual_fuel_dual_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleConsumptionProfile>))]
    public ProductSampleConsumptionProfile? DualFuelDualRate { get; init; }
}
