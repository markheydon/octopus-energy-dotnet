using System.Text.Json.Serialization;
using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Tariff snapshot for a product, region, and payment method.
/// </summary>
public sealed class ProductTariff
{
    /// <summary>
    /// Full tariff code (for example <c>E-1R-AGILE-FLEX-22-11-25-C</c>).
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; init; } = string.Empty;

    /// <summary>
    /// Parsed <see cref="TariffCode"/> when <see cref="Code"/> is a valid wire-format code;
    /// otherwise <see langword="null"/>.
    /// </summary>
    public TariffCode? ParsedTariffCode =>
        TariffCode.TryParse(Code, out TariffCode parsed) ? parsed : null;

    /// <summary>
    /// Standing charge excluding VAT, in pence per day.
    /// </summary>
    [JsonPropertyName("standing_charge_exc_vat")]
    public decimal? StandingChargeExcVat { get; init; }

    /// <summary>
    /// Standing charge including VAT, in pence per day.
    /// </summary>
    [JsonPropertyName("standing_charge_inc_vat")]
    public decimal? StandingChargeIncVat { get; init; }

    /// <summary>
    /// Standard unit rate excluding VAT, in pence per kWh.
    /// </summary>
    [JsonPropertyName("standard_unit_rate_exc_vat")]
    public decimal? StandardUnitRateExcVat { get; init; }

    /// <summary>
    /// Standard unit rate including VAT, in pence per kWh.
    /// </summary>
    [JsonPropertyName("standard_unit_rate_inc_vat")]
    public decimal? StandardUnitRateIncVat { get; init; }

    /// <summary>
    /// Day unit rate excluding VAT, in pence per kWh (dual-register electricity only).
    /// </summary>
    [JsonPropertyName("day_unit_rate_exc_vat")]
    public decimal? DayUnitRateExcVat { get; init; }

    /// <summary>
    /// Day unit rate including VAT, in pence per kWh (dual-register electricity only).
    /// </summary>
    [JsonPropertyName("day_unit_rate_inc_vat")]
    public decimal? DayUnitRateIncVat { get; init; }

    /// <summary>
    /// Night unit rate excluding VAT, in pence per kWh (dual-register electricity only).
    /// </summary>
    [JsonPropertyName("night_unit_rate_exc_vat")]
    public decimal? NightUnitRateExcVat { get; init; }

    /// <summary>
    /// Night unit rate including VAT, in pence per kWh (dual-register electricity only).
    /// </summary>
    [JsonPropertyName("night_unit_rate_inc_vat")]
    public decimal? NightUnitRateIncVat { get; init; }

    /// <summary>
    /// Online discount excluding VAT.
    /// </summary>
    [JsonPropertyName("online_discount_exc_vat")]
    public decimal? OnlineDiscountExcVat { get; init; }

    /// <summary>
    /// Online discount including VAT.
    /// </summary>
    [JsonPropertyName("online_discount_inc_vat")]
    public decimal? OnlineDiscountIncVat { get; init; }

    /// <summary>
    /// Dual-fuel discount excluding VAT.
    /// </summary>
    [JsonPropertyName("dual_fuel_discount_exc_vat")]
    public decimal? DualFuelDiscountExcVat { get; init; }

    /// <summary>
    /// Dual-fuel discount including VAT.
    /// </summary>
    [JsonPropertyName("dual_fuel_discount_inc_vat")]
    public decimal? DualFuelDiscountIncVat { get; init; }

    /// <summary>
    /// Exit fees excluding VAT.
    /// </summary>
    [JsonPropertyName("exit_fees_exc_vat")]
    public decimal? ExitFeesExcVat { get; init; }

    /// <summary>
    /// Exit fees including VAT.
    /// </summary>
    [JsonPropertyName("exit_fees_inc_vat")]
    public decimal? ExitFeesIncVat { get; init; }

    /// <summary>
    /// Exit fee type (for example <c>NONE</c>).
    /// </summary>
    [JsonPropertyName("exit_fees_type")]
    public string? ExitFeesType { get; init; }

    /// <summary>
    /// Hypermedia links to standing charges and unit-rate history endpoints.
    /// </summary>
    [JsonPropertyName("links")]
    public IReadOnlyList<ProductLink> Links { get; init; } = [];
}
