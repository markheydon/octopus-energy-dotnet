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

    /// <summary>
    /// Resolves a catalogue tariff by fuel, register kind, GSP, and payment method.
    /// </summary>
    /// <param name="fuel">Electricity or gas.</param>
    /// <param name="registerKind">Single or dual register.</param>
    /// <param name="gridSupplyPoint">UK distribution region.</param>
    /// <param name="paymentMethod">Payment method.</param>
    /// <param name="tariff">The tariff when found.</param>
    /// <returns>
    /// <see langword="true"/> when a tariff exists for the region and payment method;
    /// <see langword="false"/> when the region or payment method is absent.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="fuel"/> is gas and <paramref name="registerKind"/> is dual-register,
    /// because dual-register gas tariffs are not available in the product catalogue.
    /// </exception>
    public bool TryGetTariff(
        EnergyFuel fuel,
        TariffRegisterKind registerKind,
        GridSupplyPoint gridSupplyPoint,
        ProductPaymentMethod paymentMethod,
        out ProductTariff? tariff)
    {
        tariff = null;

        if (!TryGetPaymentMethodTariffs(fuel, registerKind, gridSupplyPoint, out ProductPaymentMethodTariffs? paymentMethodTariffs)
            || paymentMethodTariffs is null)
        {
            return false;
        }

        tariff = paymentMethodTariffs.GetTariff(paymentMethod);
        return tariff is not null;
    }

    /// <summary>
    /// Resolves a catalogue tariff using a parsed tariff code and payment method.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code (fuel, register kind, and GSP).</param>
    /// <param name="paymentMethod">Payment method.</param>
    /// <param name="tariff">The tariff when found.</param>
    /// <returns>
    /// <see langword="true"/> when a matching catalogue tariff exists for this product;
    /// <see langword="false"/> when the product code does not match, or the region or payment method is absent.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="tariffCode"/> is gas and dual-register,
    /// because dual-register gas tariffs are not available in the product catalogue.
    /// </exception>
    public bool TryGetTariff(
        TariffCode tariffCode,
        ProductPaymentMethod paymentMethod,
        out ProductTariff? tariff)
    {
        tariff = null;

        if (!string.Equals(tariffCode.ProductCode, Code, StringComparison.Ordinal))
        {
            return false;
        }

        if (!TryGetTariff(
                tariffCode.Fuel,
                tariffCode.RegisterKind,
                tariffCode.GridSupplyPoint,
                paymentMethod,
                out tariff)
            || tariff is null)
        {
            tariff = null;
            return false;
        }

        if (!string.Equals(tariff.Code, tariffCode.ToString(), StringComparison.Ordinal))
        {
            tariff = null;
            return false;
        }

        return true;
    }

    private bool TryGetPaymentMethodTariffs(
        EnergyFuel fuel,
        TariffRegisterKind registerKind,
        GridSupplyPoint gridSupplyPoint,
        out ProductPaymentMethodTariffs? paymentMethodTariffs)
    {
        paymentMethodTariffs = null;

        IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> tariffs = fuel switch
        {
            EnergyFuel.Electricity => registerKind switch
            {
                TariffRegisterKind.SingleRegister => SingleRegisterElectricityTariffs,
                TariffRegisterKind.DualRegister => DualRegisterElectricityTariffs,
                _ => throw new ArgumentOutOfRangeException(nameof(registerKind), registerKind, "Unknown register kind."),
            },
            EnergyFuel.Gas => registerKind switch
            {
                TariffRegisterKind.SingleRegister => SingleRegisterGasTariffs,
                TariffRegisterKind.DualRegister => throw new OctopusEnergyRequestException(
                    "Dual-register gas tariffs are not available in the product catalogue."),
                _ => throw new ArgumentOutOfRangeException(nameof(registerKind), registerKind, "Unknown register kind."),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(fuel), fuel, "Unknown fuel."),
        };

        if (!tariffs.TryGetValue(gridSupplyPoint, out ProductPaymentMethodTariffs? regionTariffs))
        {
            return false;
        }

        paymentMethodTariffs = regionTariffs;
        return true;
    }
}
