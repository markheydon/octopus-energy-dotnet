using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;

namespace OctopusEnergy.Client.Services.Consumption;

/// <summary>
/// Electricity and gas half-hourly (or grouped) consumption.
/// </summary>
public interface IConsumptionService
{
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
    IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
        string mpan,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

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
    /// <param name="meterPoint">Electricity meter point from <see cref="Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPAN or <paramref name="meterSerialNumber"/> is null or whitespace.
    /// </exception>
    IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
        ElectricityMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches one REST page of electricity consumption for an MPAN and meter serial.
    /// </summary>
    /// <param name="mpan">13-digit MPAN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
        string mpan,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches one REST page of electricity consumption for an account meter point and meter serial.
    /// </summary>
    /// <param name="meterPoint">Electricity meter point from <see cref="Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPAN or <paramref name="meterSerialNumber"/> is null or whitespace.
    /// </exception>
    Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
        ElectricityMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

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
    IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
        string mprn,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches one REST page of gas consumption for an MPRN and meter serial.
    /// </summary>
    /// <param name="mprn">Gas MPRN.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
        string mprn,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches one REST page of gas consumption for an account meter point and meter serial.
    /// </summary>
    /// <param name="meterPoint">Gas meter point from <see cref="Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A single page including <see cref="PaginatedResult{TItem}.Count"/>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPRN or <paramref name="meterSerialNumber"/> is null or whitespace.
    /// </exception>
    Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
        GasMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);

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
    /// <param name="meterPoint">Gas meter point from <see cref="Accounts.AccountService"/>.</param>
    /// <param name="meterSerialNumber">Meter serial number.</param>
    /// <param name="request">Optional period, pagination, ordering, and grouping.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consumption intervals across pages. An empty sequence is valid for non-smart meters.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="meterPoint"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="meterPoint"/> has no MPRN or <paramref name="meterSerialNumber"/> is null or whitespace.
    /// </exception>
    IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
        GasMeterPoint meterPoint,
        string meterSerialNumber,
        ConsumptionListRequest? request = null,
        CancellationToken cancellationToken = default);
}
