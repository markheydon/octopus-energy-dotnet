using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Tariffs for a grid supply point, keyed by payment method.
/// </summary>
public sealed class ProductPaymentMethodTariffs
{
    /// <summary>
    /// Direct debit monthly payment method.
    /// </summary>
    [JsonPropertyName("direct_debit_monthly")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductTariff>))]
    public ProductTariff? DirectDebitMonthly { get; init; }

    /// <summary>
    /// Direct debit quarterly payment method.
    /// </summary>
    [JsonPropertyName("direct_debit_quarterly")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductTariff>))]
    public ProductTariff? DirectDebitQuarterly { get; init; }

    /// <summary>
    /// Non-direct-debit payment method.
    /// </summary>
    [JsonPropertyName("non_direct_debit")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductTariff>))]
    public ProductTariff? NonDirectDebit { get; init; }
}
