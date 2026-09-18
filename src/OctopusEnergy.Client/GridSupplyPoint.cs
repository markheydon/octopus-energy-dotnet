using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client;

/// <summary>
/// UK electricity distribution region (GSP group) encoded as a single letter in tariff codes.
/// </summary>
/// <remarks>
/// Letter <c>I</c> is not used. Product JSON and industry lookup responses use a leading underscore
/// (for example <c>_C</c> for London).
/// </remarks>
[JsonConverter(typeof(GridSupplyPointJsonConverter))]
public enum GridSupplyPoint
{
    /// <summary>Eastern England.</summary>
    A,

    /// <summary>East Midlands.</summary>
    B,

    /// <summary>London.</summary>
    C,

    /// <summary>Merseyside and North Wales.</summary>
    D,

    /// <summary>West Midlands.</summary>
    E,

    /// <summary>North Eastern England.</summary>
    F,

    /// <summary>North Western England.</summary>
    G,

    /// <summary>Southern England.</summary>
    H,

    /// <summary>South Eastern England.</summary>
    J,

    /// <summary>South Wales.</summary>
    K,

    /// <summary>South Western England.</summary>
    L,

    /// <summary>Yorkshire.</summary>
    M,

    /// <summary>Southern Scotland.</summary>
    N,

    /// <summary>Northern Scotland.</summary>
    P,
}
