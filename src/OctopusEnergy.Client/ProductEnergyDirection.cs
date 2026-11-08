namespace OctopusEnergy.Client;

/// <summary>
/// Import or export direction on a catalogue product.
/// </summary>
/// <remarks>
/// Known Octopus wire tokens: <c>IMPORT</c>, <c>EXPORT</c> (case-insensitive).
/// </remarks>
public enum ProductEnergyDirection
{
    /// <summary>
    /// Unrecognised or missing wire token.
    /// </summary>
    Unknown,

    /// <summary>
    /// Import product (<c>IMPORT</c>).
    /// </summary>
    Import,

    /// <summary>
    /// Export product (<c>EXPORT</c>).
    /// </summary>
    Export,
}
