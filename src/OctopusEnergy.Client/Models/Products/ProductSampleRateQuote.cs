using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Sample quote figures for a payment method at a grid supply point.
/// </summary>
public sealed class ProductSampleRateQuote
{
    /// <summary>
    /// Single-rate electricity sample quote.
    /// </summary>
    [JsonPropertyName("electricity_single_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleAnnualCost>))]
    public ProductSampleAnnualCost? ElectricitySingleRate { get; init; }

    /// <summary>
    /// Dual-rate electricity sample quote.
    /// </summary>
    [JsonPropertyName("electricity_dual_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleAnnualCost>))]
    public ProductSampleAnnualCost? ElectricityDualRate { get; init; }

    /// <summary>
    /// Dual-fuel single-rate sample quote.
    /// </summary>
    [JsonPropertyName("dual_fuel_single_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleAnnualCost>))]
    public ProductSampleAnnualCost? DualFuelSingleRate { get; init; }

    /// <summary>
    /// Dual-fuel dual-rate sample quote.
    /// </summary>
    [JsonPropertyName("dual_fuel_dual_rate")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleAnnualCost>))]
    public ProductSampleAnnualCost? DualFuelDualRate { get; init; }
}
