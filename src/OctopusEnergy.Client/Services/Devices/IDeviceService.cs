using OctopusEnergy.Client.Models.Devices;

namespace OctopusEnergy.Client.Services.Devices;

/// <summary>
/// GraphQL SmartFlex device discovery and planned dispatch windows for the authenticated dashboard API key.
/// </summary>
public interface IDeviceService
{
    /// <summary>
    /// Lists SmartFlex devices registered on the account.
    /// </summary>
    /// <param name="accountNumber">Account number (for example <c>A-12345678</c>).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Devices linked to the account. The list is empty when the customer has no enrolled hardware;
    /// it is never null.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the account number is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c>.
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised.
    /// </exception>
    Task<IReadOnlyList<SmartFlexDevice>> ListDevicesAsync(
        string accountNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists planned dispatch windows for a SmartFlex device in time order.
    /// </summary>
    /// <param name="deviceId">Device id from <see cref="ListDevicesAsync"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Planned dispatches. The list is empty when none are scheduled; it is never null.
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or the device id is missing.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c> (for example unauthorised device).
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised.
    /// </exception>
    Task<IReadOnlyList<FlexPlannedDispatch>> ListFlexPlannedDispatchesAsync(
        string deviceId,
        CancellationToken cancellationToken = default);
}
