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
    /// Lists gas consumption for an MPRN and meter serial.
    /// </summary>
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
}
