using System.Text.Json.Serialization;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Sample quote figures for a grid supply point, keyed by payment method.
/// </summary>
public sealed class ProductSampleQuotesByPaymentMethod
{
    /// <summary>
    /// Direct debit monthly sample quotes.
    /// </summary>
    [JsonPropertyName("direct_debit_monthly")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleRateQuote>))]
    public ProductSampleRateQuote? DirectDebitMonthly { get; init; }

    /// <summary>
    /// Direct debit quarterly sample quotes.
    /// </summary>
    [JsonPropertyName("direct_debit_quarterly")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleRateQuote>))]
    public ProductSampleRateQuote? DirectDebitQuarterly { get; init; }

    /// <summary>
    /// Non-direct-debit sample quotes.
    /// </summary>
    [JsonPropertyName("non_direct_debit")]
    [JsonConverter(typeof(EmptyObjectAsNullConverter<ProductSampleRateQuote>))]
    public ProductSampleRateQuote? NonDirectDebit { get; init; }
}
