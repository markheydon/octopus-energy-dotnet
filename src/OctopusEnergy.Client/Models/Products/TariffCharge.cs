using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// A standing charge or unit-rate period for a tariff.
/// </summary>
public sealed class TariffCharge
{
    /// <summary>
    /// Charge value excluding VAT, in pence per kWh or pence per day.
    /// </summary>
    [JsonPropertyName("value_exc_vat")]
    public decimal ValueExcVat { get; init; }

    /// <summary>
    /// Charge value including VAT, in pence per kWh or pence per day.
    /// </summary>
    [JsonPropertyName("value_inc_vat")]
    public decimal ValueIncVat { get; init; }

    /// <summary>
    /// When this charge period started.
    /// </summary>
    [JsonPropertyName("valid_from")]
    public DateTimeOffset ValidFrom { get; init; }

    /// <summary>
    /// When this charge period ended.
    /// </summary>
    [JsonPropertyName("valid_to")]
    public DateTimeOffset ValidTo { get; init; }

    /// <summary>
    /// Payment method when returned by the API, otherwise <see langword="null"/>.
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; init; }
}
