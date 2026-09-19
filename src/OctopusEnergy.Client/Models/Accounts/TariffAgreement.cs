using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.Accounts;

/// <summary>
/// A tariff agreement on a meter point.
/// </summary>
public sealed class TariffAgreement
{
    private OctopusEnergy.Client.TariffCode? _parsedTariffCode;
    private bool _parsedTariffCodeInitialized;

    /// <summary>
    /// Tariff code (for example <c>E-1R-VAR-22-11-01-N</c>).
    /// </summary>
    [JsonPropertyName("tariff_code")]
    public string TariffCode { get; init; } = string.Empty;

    /// <summary>
    /// Parsed tariff code when <see cref="TariffCode"/> is a valid wire-format string;
    /// otherwise <see langword="null"/>.
    /// </summary>
    public OctopusEnergy.Client.TariffCode? ParsedTariffCode
    {
        get
        {
            if (_parsedTariffCodeInitialized)
            {
                return _parsedTariffCode;
            }

            _parsedTariffCodeInitialized = true;
            _parsedTariffCode = OctopusEnergy.Client.TariffCode.TryParse(
                TariffCode,
                out OctopusEnergy.Client.TariffCode parsed)
                ? parsed
                : null;
            return _parsedTariffCode;
        }
    }

    /// <summary>
    /// When the agreement started.
    /// </summary>
    [JsonPropertyName("valid_from")]
    public DateTimeOffset ValidFrom { get; init; }

    /// <summary>
    /// When the agreement ended, or <see langword="null"/> if still active.
    /// </summary>
    [JsonPropertyName("valid_to")]
    public DateTimeOffset? ValidTo { get; init; }
}
