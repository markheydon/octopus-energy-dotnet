namespace OctopusEnergy.Client;

/// <summary>
/// Register count encoded in an Octopus tariff code.
/// </summary>
public enum TariffRegisterKind
{
    /// <summary>
    /// Single register (<c>1R</c>).
    /// </summary>
    SingleRegister,

    /// <summary>
    /// Dual register / Economy 7 (<c>2R</c>).
    /// </summary>
    DualRegister,
}
