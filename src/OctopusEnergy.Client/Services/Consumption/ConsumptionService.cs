using System.Globalization;
using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;

namespace OctopusEnergy.Client.Services.Consumption;

/// <summary>
/// Electricity and gas half-hourly (or grouped) consumption.
/// </summary>
public sealed class ConsumptionService : IConsumptionService
{
    private const string ElectricityMeterPointsPathPrefix = "electricity-meter-points/";
    private const string GasMeterPointsPathPrefix = "gas-meter-points/";

    private readonly RestClient _rest;

    internal ConsumptionService(RestClient rest)
    {
        ArgumentNullException.ThrowIfNull(rest);
        _rest = rest;
    }

    /// <summary>
    /// Lists electricity consumption for an MPAN and meter serial, including export MPANs.
    /// </summary>
    /// <remarks>
    /// This method follows every <c>next</c> link until the list is exhausted. Without
    /// <see cref="ConsumptionListRequest.PeriodFrom"/> and <see cref="ConsumptionListRequest.PeriodTo"/>,
    /// that can require many HTTP requests for long histories. Prefer
    /// <see cref="ListElectricityPageAsync(string, string, ConsumptionListRequest?, CancellationToken)"/>
    /// when you need a single page or the total <c>count</c> without auto-pagination.
    /// </remarks>
    /// <param name="mpan">13-digit MPAN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    public IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
        string mpan,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ValidateMeterIdentifiers(mpan, meterSerialNumber);

        string path = BuildPath(
            $"{ElectricityMeterPointsPathPrefix}{Uri.EscapeDataString(mpan)}/meters/{Uri.EscapeDataString(meterSerialNumber)}/consumption/",
            request);

        return _rest.GetAllPagesAsync<ConsumptionInterval>(path, cancellationToken);
    }

    /// <summary>
    /// Fetches one REST page of electricity consumption for an MPAN and meter serial.
    /// </summary>
    /// <param name="mpan">13-digit MPAN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    public Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
        string mpan,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ValidateMeterIdentifiers(mpan, meterSerialNumber);

        string path = BuildPath(
            $"{ElectricityMeterPointsPathPrefix}{Uri.EscapeDataString(mpan)}/meters/{Uri.EscapeDataString(meterSerialNumber)}/consumption/",
            request);

        return _rest.GetPageAsync<ConsumptionInterval>(path, cancellationToken);
    }

    /// <summary>
    /// Fetches one REST page of electricity consumption for an account meter point and meter serial.
    /// </summary>
    /// <param name="meterPoint">Electricity meter point from <see cref="Services.Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPAN.
    /// </exception>
    public Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
        ElectricityMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meterPoint);

        if (string.IsNullOrWhiteSpace(meterPoint.Mpan))
        {
            throw new OctopusEnergyRequestException("Meter point MPAN is required.");
        }

        return ListElectricityPageAsync(meterPoint.Mpan, meterSerialNumber, request, cancellationToken);
    }

    /// <summary>
    /// Lists electricity consumption for an account meter point and meter serial, including export MPANs.
    /// </summary>
    /// <remarks>
    /// This method follows every <c>next</c> link until the list is exhausted. Without
    /// <see cref="ConsumptionListRequest.PeriodFrom"/> and <see cref="ConsumptionListRequest.PeriodTo"/>,
    /// that can require many HTTP requests for long histories. Prefer
    /// <see cref="ListElectricityPageAsync(ElectricityMeterPoint, string, ConsumptionListRequest?, CancellationToken)"/>
    /// when you need a single page or the total <c>count</c> without auto-pagination.
    /// </remarks>
    /// <param name="meterPoint">Electricity meter point from <see cref="Services.Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPAN.
    /// </exception>
    public IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
        ElectricityMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meterPoint);

        if (string.IsNullOrWhiteSpace(meterPoint.Mpan))
        {
            throw new OctopusEnergyRequestException("Meter point MPAN is required.");
        }

        return ListElectricityAsync(meterPoint.Mpan, meterSerialNumber, request, cancellationToken);
    }

    /// <summary>
    /// Lists gas consumption for an MPRN and meter serial.
    /// </summary>
    /// <remarks>
    /// This method follows every <c>next</c> link until the list is exhausted. Without
    /// <see cref="ConsumptionListRequest.PeriodFrom"/> and <see cref="ConsumptionListRequest.PeriodTo"/>,
    /// that can require many HTTP requests for long histories. Prefer
    /// <see cref="ListGasPageAsync(string, string, ConsumptionListRequest?, CancellationToken)"/>
    /// when you need a single page or the total <c>count</c> without auto-pagination.
    /// </remarks>
    /// <param name="mprn">Gas MPRN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    public IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
        string mprn,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ValidateMeterIdentifiers(mprn, meterSerialNumber);

        string path = BuildPath(
            $"{GasMeterPointsPathPrefix}{Uri.EscapeDataString(mprn)}/meters/{Uri.EscapeDataString(meterSerialNumber)}/consumption/",
            request);

        return _rest.GetAllPagesAsync<ConsumptionInterval>(path, cancellationToken);
    }

    /// <summary>
    /// Fetches one REST page of gas consumption for an MPRN and meter serial.
    /// </summary>
    /// <param name="mprn">Gas MPRN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    public Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
        string mprn,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ValidateMeterIdentifiers(mprn, meterSerialNumber);

        string path = BuildPath(
            $"{GasMeterPointsPathPrefix}{Uri.EscapeDataString(mprn)}/meters/{Uri.EscapeDataString(meterSerialNumber)}/consumption/",
            request);

        return _rest.GetPageAsync<ConsumptionInterval>(path, cancellationToken);
    }

    /// <summary>
    /// Fetches one REST page of gas consumption for an account meter point and meter serial.
    /// </summary>
    /// <param name="meterPoint">Gas meter point from <see cref="Services.Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPRN.
    /// </exception>
    public Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
        GasMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meterPoint);

        if (string.IsNullOrWhiteSpace(meterPoint.Mprn))
        {
            throw new OctopusEnergyRequestException("Meter point MPRN is required.");
        }

        return ListGasPageAsync(meterPoint.Mprn, meterSerialNumber, request, cancellationToken);
    }

    /// <summary>
    /// Lists gas consumption for an account meter point and meter serial.
    /// </summary>
    /// <remarks>
    /// This method follows every <c>next</c> link until the list is exhausted. Without
    /// <see cref="ConsumptionListRequest.PeriodFrom"/> and <see cref="ConsumptionListRequest.PeriodTo"/>,
    /// that can require many HTTP requests for long histories. Prefer
    /// <see cref="ListGasPageAsync(GasMeterPoint, string, ConsumptionListRequest?, CancellationToken)"/>
    /// when you need a single page or the total <c>count</c> without auto-pagination.
    /// </remarks>
    /// <param name="meterPoint">Gas meter point from <see cref="Services.Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPRN.
    /// </exception>
    public IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
        GasMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(meterPoint);

        if (string.IsNullOrWhiteSpace(meterPoint.Mprn))
        {
            throw new OctopusEnergyRequestException("Meter point MPRN is required.");
        }

        return ListGasAsync(meterPoint.Mprn, meterSerialNumber, request, cancellationToken);
    }

    private static void ValidateMeterIdentifiers(string meterPointIdentifier, string meterSerialNumber)
    {
        if (string.IsNullOrWhiteSpace(meterPointIdentifier))
        {
            throw new OctopusEnergyRequestException("Meter point identifier is required.");
        }

        if (string.IsNullOrWhiteSpace(meterSerialNumber))
        {
            throw new OctopusEnergyRequestException("Meter serial number is required.");
        }
    }

    private static string BuildPath(string relativePath, ConsumptionListRequest? request)
    {
        if (request is null)
        {
            return relativePath;
        }

        if (request.PageSize is not null)
        {
            RestPageSizeLimits.Validate(request.PageSize.Value, RestPageSizeLimits.ConsumptionMaximum);
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

        if (request.OrderBy is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "order_by",
                FormatOrderBy(request.OrderBy.Value)));
        }

        if (request.GroupBy is not null)
        {
            parameters.Add(new RestQuery.QueryParameter(
                "group_by",
                FormatGroupBy(request.GroupBy.Value)));
        }

        return RestQuery.Append(relativePath, parameters);
    }

    private static string FormatOrderBy(ConsumptionOrderBy orderBy)
    {
        return orderBy switch
        {
            ConsumptionOrderBy.PeriodAscending => "period",
            ConsumptionOrderBy.PeriodDescending => "-period",
            _ => throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, "Unknown order."),
        };
    }

    private static string FormatGroupBy(ConsumptionGroupBy groupBy)
    {
        return groupBy switch
        {
            ConsumptionGroupBy.Hour => "hour",
            ConsumptionGroupBy.Day => "day",
            ConsumptionGroupBy.Week => "week",
            ConsumptionGroupBy.Month => "month",
            ConsumptionGroupBy.Quarter => "quarter",
            _ => throw new ArgumentOutOfRangeException(nameof(groupBy), groupBy, "Unknown group."),
        };
    }
}
