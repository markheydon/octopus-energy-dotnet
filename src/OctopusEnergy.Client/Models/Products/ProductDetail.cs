using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Product detail including tariffs by grid supply point and payment method.
/// </summary>
public sealed class ProductDetail : Product
{
    private IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> _singleRegisterElectricityTariffs =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    private IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> _dualRegisterElectricityTariffs =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    private IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> _singleRegisterGasTariffs =
        new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();

    private IReadOnlyDictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod> _sampleQuotes =
        new Dictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod>();

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
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> SingleRegisterElectricityTariffs
    {
        get => _singleRegisterElectricityTariffs;
        init => _singleRegisterElectricityTariffs = value ?? new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();
    }

    /// <summary>
    /// Dual-register electricity tariffs by GSP and payment method.
    /// </summary>
    [JsonPropertyName("dual_register_electricity_tariffs")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductPaymentMethodTariffs>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> DualRegisterElectricityTariffs
    {
        get => _dualRegisterElectricityTariffs;
        init => _dualRegisterElectricityTariffs = value ?? new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();
    }

    /// <summary>
    /// Single-register gas tariffs by GSP and payment method.
    /// </summary>
    [JsonPropertyName("single_register_gas_tariffs")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductPaymentMethodTariffs>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> SingleRegisterGasTariffs
    {
        get => _singleRegisterGasTariffs;
        init => _singleRegisterGasTariffs = value ?? new Dictionary<GridSupplyPoint, ProductPaymentMethodTariffs>();
    }

    /// <summary>
    /// Sample annual cost quotes by GSP and payment method.
    /// </summary>
    [JsonPropertyName("sample_quotes")]
    [JsonConverter(typeof(GridSupplyPointDictionaryConverter<ProductSampleQuotesByPaymentMethod>))]
    public IReadOnlyDictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod> SampleQuotes
    {
        get => _sampleQuotes;
        init => _sampleQuotes = value ?? new Dictionary<GridSupplyPoint, ProductSampleQuotesByPaymentMethod>();
    }

    /// <summary>
    /// Sample consumption figures used for quotes.
    /// </summary>
    [JsonPropertyName("sample_consumption")]
    public ProductSampleConsumption? SampleConsumption { get; init; }
}
