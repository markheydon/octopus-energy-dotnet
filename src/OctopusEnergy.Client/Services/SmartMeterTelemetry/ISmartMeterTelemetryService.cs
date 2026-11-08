using OctopusEnergy.Client.Models.SmartMeterTelemetry;
using OctopusEnergy.Client.Services.RateLimits;

namespace OctopusEnergy.Client.Services.SmartMeterTelemetry;

/// <summary>
/// GraphQL Home Mini / CAD <c>smartMeterTelemetry</c> and EUI64 discovery for the authenticated dashboard API key.
/// </summary>
public interface ISmartMeterTelemetryService
{
    /// <summary>
    /// Lists SMETS devices with EUI64 ids discoverable on the account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Devices deduplicated by <see cref="SmartMeterDevice.DeviceId"/>.
    /// The list is empty when no smart devices are linked; it is never null.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the account number is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or the account node is missing.
    /// </exception>
    /// <remarks>
    /// Uses the curated path <c>account → electricityAgreements | gasAgreements → meterPoint → meters → smartDevices</c>.
    /// </remarks>
    Task<IReadOnlyList<SmartMeterDevice>> ListSmartDevicesAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the latest telemetry snapshot for a device.
    /// </summary>
    /// <param name="deviceId">EUI64 from <see cref="ListSmartDevicesAsync"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Telemetry samples. The list is empty when none are available; it is never null.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the device id is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>
    /// (including field rate limits <c>KT-CT-1199</c>). The SDK does not retry automatically.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised.
    /// </exception>
    Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
        string deviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches telemetry for a time range at the requested granularity.
    /// </summary>
    /// <param name="deviceId">EUI64 from <see cref="ListSmartDevicesAsync"/>.</param>
    /// <param name="range">Inclusive start, exclusive end, and grouping (all required).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Telemetry samples. The list is empty when none are available; it is never null.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the device id is missing.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="range"/> is null.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>
    /// (including field rate limits <c>KT-CT-1199</c>). The SDK does not retry automatically.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised.
    /// </exception>
    /// <remarks>
    /// Field rate limits are aggressive (on the order of 125 calls per hour per user for this query).
    /// Back off in application code; see product documentation and
    /// <see cref="RateLimits.IRateLimitService.GetRateLimitInfoAsync(System.Threading.CancellationToken)"/>.
    /// </remarks>
    Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
        string deviceId,
        SmartMeterTelemetryRange range,
        CancellationToken cancellationToken = default);
}
