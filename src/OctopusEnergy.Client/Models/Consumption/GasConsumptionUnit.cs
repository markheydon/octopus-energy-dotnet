namespace OctopusEnergy.Client.Models.Consumption;

/// <summary>
/// Gas consumption units returned by the API.
/// </summary>
public enum GasConsumptionUnit
{
    /// <summary>Unit not returned or not recognised.</summary>
    Unknown,

    /// <summary>SMETS1 gas consumption in kilowatt-hours.</summary>
    KilowattHours,

    /// <summary>SMETS2 gas consumption in cubic metres.</summary>
    CubicMetres,
}
