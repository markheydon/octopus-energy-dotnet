using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Product detail including tariffs by grid supply point and payment method.
/// </summary>
public sealed class ProductDetail : Product
{
    /// <summary>
    /// Timestamp for which tariff snapshots are active.
    /// </summary>
    [JsonPropertyName("tariffs_active_at")]
    public DateTimeOffset? TariffsActiveAt { get; init; }

    /// <summary>
    /// Single-register electricity tariffs by GSP and payment method.
    /// </summary>
    [JsonPropertyName("single_register_electricity_tariffs")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductPaymentMethodTariffs>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> SingleRegisterElectricityTariffs { get; init; } =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    /// <summary>
    /// Dual-register electricity tariffs by GSP and payment method.
    /// </summary>
    [JsonPropertyName("dual_register_electricity_tariffs")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductPaymentMethodTariffs>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> DualRegisterElectricityTariffs { get; init; } =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    /// <summary>
    /// Single-register gas tariffs by GSP and payment method.
    /// </summary>
    [JsonPropertyName("single_register_gas_tariffs")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductPaymentMethodTariffs>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> SingleRegisterGasTariffs { get; init; } =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    /// <summary>
    /// Sample annual cost quotes by GSP and payment method.
    /// </summary>
    [JsonPropertyName("sample_quotes")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductSampleQuotesByPaymentMethod>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod> SampleQuotes { get; init; } =
        new Dictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod>();

    /// <summary>
    /// Sample consumption figures used for quotes.
    /// </summary>
    [JsonPropertyName("sample_consumption")]
    public ProductSampleConsumption? SampleConsumption { get; init; }
}
