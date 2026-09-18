using System.Globalization;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Services.Products;

/// <summary>
/// Standing charges and unit rates for product tariffs.
/// </summary>
public sealed class TariffRatesService
{
    private readonly RestClient _rest;

    internal TariffRatesService(RestClient rest)
    {
        ArgumentNullException.ThrowIfNull(rest);
        _rest = rest;
    }

    /// <summary>
    /// Lists standing charges for a tariff, following REST pagination automatically.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IAsyncEnumerable<TariffCharge> ListStandingChargesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return ListChargesAsync(tariffCode, TariffChargeKind.StandingCharges, request, cancellationToken);
    }

    /// <summary>
    /// Lists standard unit rates for a tariff, following REST pagination automatically.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IAsyncEnumerable<TariffCharge> ListStandardUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return ListChargesAsync(tariffCode, TariffChargeKind.StandardUnitRates, request, cancellationToken);
    }

    /// <summary>
    /// Lists day unit rates for a dual-register electricity tariff.
    /// </summary>
    /// <param name="tariffCode">Parsed dual-register electricity tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="tariffCode"/> is gas or single-register electricity.
    /// </exception>
    public IAsyncEnumerable<TariffCharge> ListDayUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return ListChargesAsync(tariffCode, TariffChargeKind.DayUnitRates, request, cancellationToken);
    }

    /// <summary>
    /// Lists night unit rates for a dual-register electricity tariff.
    /// </summary>
    /// <param name="tariffCode">Parsed dual-register electricity tariff code.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="tariffCode"/> is gas or single-register electricity.
    /// </exception>
    public IAsyncEnumerable<TariffCharge> ListNightUnitRatesAsync(
        TariffCode tariffCode,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        return ListChargesAsync(tariffCode, TariffChargeKind.NightUnitRates, request, cancellationToken);
    }

    /// <summary>
    /// Lists charge periods for a tariff charge endpoint.
    /// </summary>
    /// <param name="tariffCode">Parsed tariff code.</param>
    /// <param name="chargeKind">Standing charges or unit-rate list.</param>
    /// <param name="request">Optional period and page-size filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public IAsyncEnumerable<TariffCharge> ListChargesAsync(
        TariffCode tariffCode,
        TariffChargeKind chargeKind,
        TariffChargeListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        string path = BuildPath(tariffCode, chargeKind, request);
        return _rest.GetAllPagesAsync<TariffCharge>(path, cancellationToken);
    }

    private static string BuildPath(
        TariffCode tariffCode,
        TariffChargeKind chargeKind,
        TariffChargeListRequest? request)
    {
        string path = tariffCode.GetRelativeChargePath(chargeKind);

        if (request is null)
        {
            return path;
        }

        if (request.PageSize is not null)
        {
            RestPageSizeLimits.Validate(request.PageSize.Value, RestPageSizeLimits.RatesMaximum);
        }

        List<RestQuery.QueryParameter> parameters = [];

        if (request.PeriodFrom is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "period_from",
                RestQuery.FormatDateTimeOffset(request.PeriodFrom.Value)));
        }

        if (request.PeriodTo is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "period_to",
                RestQuery.FormatDateTimeOffset(request.PeriodTo.Value)));
        }

        if (request.PageSize is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "page_size",
                request.PageSize.Value.ToString(CultureInfo.InvariantCulture)));
        }

        return RestQuery.Append(path, parameters);
    }
}
