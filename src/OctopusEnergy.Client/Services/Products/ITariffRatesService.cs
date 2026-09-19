using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Services.Products;

/// <summary>
/// Standing charges and unit rates for product tariffs.
/// </summary>
public interface ITariffRatesService
{
    /// <summary>
    /// Lists standing charges for a tariff, following REST pagination automatically.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    IAsyncEnumerable<TariffCharge> ListStandingChargesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists standard unit rates for a tariff, following REST pagination automatically.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    IAsyncEnumerable<TariffCharge> ListStandardUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists day unit rates for a dual-register electricity tariff.
    /// </summary>
    /// <param name="tariffCode">Parsed dual-register electricity tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="tariffCode"/> is gas or single-register electricity.
    /// </exception>
    IAsyncEnumerable<TariffCharge> ListDayUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists night unit rates for a dual-register electricity tariff.
    /// </summary>
    /// <param name="tariffCode">Parsed dual-register electricity tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="tariffCode"/> is gas or single-register electricity.
    /// </exception>
    IAsyncEnumerable<TariffCharge> ListNightUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists charge periods for a tariff charge endpoint.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="chargeKind">Standing charges or unit-rate list.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    IAsyncEnumerable<TariffCharge> ListChargesAsync(
        TariffCode tariffCode,
        TariffChargeKind chargeKind,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default);
}
