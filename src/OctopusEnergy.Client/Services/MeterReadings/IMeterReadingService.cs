using OctopusEnergy.Client.Models.MeterReadings;

namespace OctopusEnergy.Client.Services.MeterReadings;

/// <summary>
/// GraphQL meter reading mutations and consent lookup for the authenticated dashboard API key.
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

    /// <summary>
    /// Submits a gas meter reading for the given MPRN and serial number.
    /// </summary>
    /// <param name="request">Reading details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The mutation payload. Inspect <see cref="GasMeterReadingResult.ReadingErrors"/> for field-level validation issues.
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
    /// <remarks>
    /// Runtime allow-list status for <c>createGasMeterReading</c> is <strong>Not probed</strong> on the maintainer account
    /// (no gas meter). Behaviour is covered by schema-aligned fixtures; re-probe with a gas-capable dashboard key before
    /// treating the allow-list row as <strong>Allowed (mutation)</strong>.
    /// </remarks>
    Task<GasMeterReadingResult> CreateGasAsync(
        GasMeterReadingCreate request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the reading consent granularity for an electricity MPAN.
    /// </summary>
    /// <param name="mpan">Import MPAN for the meter point.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Consent granularity returned by Kraken.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or <paramref name="mpan"/> is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or is missing expected data.
    /// </exception>
    Task<ReadingConsentGranularity> GetReadingConsentGranularityAsync(
        string mpan,
        CancellationToken cancellationToken = default);
}
