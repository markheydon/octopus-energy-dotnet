namespace OctopusEnergy.Client.Models.Devices;

/// <summary>
/// A planned SmartFlex dispatch window for a device.
/// </summary>
public sealed class FlexPlannedDispatch
{
    /// <summary>
    /// Gets the start of the dispatch window.
    /// </summary>
    public DateTimeOffset Start { get; init; }

    /// <summary>
    /// Gets the end of the dispatch window.
    /// </summary>
    public DateTimeOffset End { get; init; }

    /// <summary>
    /// Gets the charging type (for example smart or boost modes) when returned by the API.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the planned energy change in kWh. Negative values indicate export or discharge slots.
    /// </summary>
    public decimal? EnergyAddedKwh { get; init; }
}
