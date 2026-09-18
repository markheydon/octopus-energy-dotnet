using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Sample annual cost figures for a tariff quote.
/// </summary>
public sealed class ProductSampleAnnualCost
{
    /// <summary>
    /// Annual cost excluding VAT, in pence.
    /// </summary>
    [JsonPropertyName("annual_cost_exc_vat")]
    public decimal? AnnualCostExcVat { get; init; }

    /// <summary>
    /// Annual cost including VAT, in pence.
    /// </summary>
    [JsonPropertyName("annual_cost_inc_vat")]
    public decimal? AnnualCostIncVat { get; init; }
}
