namespace OctopusEnergy.Client.Models.Devices;

/// <summary>
/// A SmartFlex or Intelligent device enrolled on the customer account.
/// </summary>
public sealed class SmartFlexDevice
{
    /// <summary>
    /// Gets the Kraken device identifier (UUID).
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets the user-facing device name when the API supplies one.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets the Kraken Flex device type when returned by the API.
    /// </summary>
    public string? DeviceType { get; init; }

    /// <summary>
    /// Gets the integration provider when returned by the API.
    /// </summary>
    public string? Provider { get; init; }

    /// <summary>
    /// Gets the GraphQL concrete type name (for example <c>SmartFlexVehicle</c>).
    /// </summary>
    public string? TypeName { get; init; }

    /// <summary>
    /// Gets the vehicle or charge point make when applicable.
    /// </summary>
    public string? Make { get; init; }

    /// <summary>
    /// Gets the vehicle or charge point model when applicable.
    /// </summary>
    public string? Model { get; init; }

    /// <summary>
    /// Gets the current operational state from <c>status.currentState</c> when present.
    /// </summary>
    public string? CurrentState { get; init; }
}
