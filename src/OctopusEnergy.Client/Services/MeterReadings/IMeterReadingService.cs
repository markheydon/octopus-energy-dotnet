using OctopusEnergy.Client.Models.MeterReadings;

namespace OctopusEnergy.Client.Services.MeterReadings;

/// <summary>
/// GraphQL meter reading mutations for the authenticated dashboard API key.
/// </summary>
public interface IMeterReadingService
{
    /// <summary>
    /// Submits an electricity meter reading for the given MPAN and serial number.
    /// </summary>
    /// <param name="request">Reading details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The mutation payload. Inspect <see cref="ElectricityMeterReadingResult.ReadingErrors"/> for field-level validation issues.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or request fields fail local validation.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or is missing expected data.
    /// </exception>
    Task<ElectricityMeterReadingResult> CreateElectricityAsync(
        ElectricityMeterReadingCreate request,
        CancellationToken cancellationToken = default);
}
