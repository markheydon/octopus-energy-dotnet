namespace OctopusEnergy.Client;

/// <summary>
/// Parses and formats UK grid supply point (GSP) identifiers.
/// </summary>
public static class GridSupplyPointParser
{
    /// <summary>
    /// Returns the product JSON / industry <c>group_id</c> for a GSP (for example <c>_C</c>).
    /// </summary>
    /// <param name="supplyPoint">The grid supply point.</param>
    /// <returns>The underscore-prefixed group id.</returns>
    public static string ToGroupId(GridSupplyPoint supplyPoint)
    {
        return $"_{ToLetter(supplyPoint)}";
    }

    /// <summary>
    /// Returns the single-letter suffix used in tariff codes.
    /// </summary>
    /// <param name="supplyPoint">The grid supply point.</param>
    /// <returns>The GSP letter.</returns>
    public static char ToLetter(GridSupplyPoint supplyPoint)
    {
        return supplyPoint switch
        {
            GridSupplyPoint.A => 'A',
            GridSupplyPoint.B => 'B',
            GridSupplyPoint.C => 'C',
            GridSupplyPoint.D => 'D',
            GridSupplyPoint.E => 'E',
            GridSupplyPoint.F => 'F',
            GridSupplyPoint.G => 'G',
            GridSupplyPoint.H => 'H',
            GridSupplyPoint.J => 'J',
            GridSupplyPoint.K => 'K',
            GridSupplyPoint.L => 'L',
            GridSupplyPoint.M => 'M',
            GridSupplyPoint.N => 'N',
            GridSupplyPoint.P => 'P',
            _ => throw new ArgumentOutOfRangeException(nameof(supplyPoint), supplyPoint, "Unknown grid supply point."),
        };
    }

    /// <summary>
    /// Parses a GSP from a tariff-code suffix letter or a <c>group_id</c> such as <c>_C</c> or <c>C</c>.
    /// </summary>
    /// <param name="value">The letter or group id to parse.</param>
    /// <returns>The grid supply point.</returns>
    /// <exception cref="OctopusEnergyRequestException">The value is not a valid GSP identifier.</exception>
    public static GridSupplyPoint Parse(string value)
    {
        if (!TryParse(value, out GridSupplyPoint supplyPoint))
        {
            throw new OctopusEnergyRequestException(
                $"The value '{value}' is not a valid grid supply point. Expected a letter A–P (excluding I) or a group id such as _C.");
        }

        return supplyPoint;
    }

    /// <summary>
    /// Attempts to parse a GSP from a tariff-code suffix letter or a <c>group_id</c>.
    /// </summary>
    /// <param name="value">The letter or group id to parse.</param>
    /// <param name="supplyPoint">The parsed grid supply point when successful.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(string? value, out GridSupplyPoint supplyPoint)
    {
        supplyPoint = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string trimmed = value.Trim();

        if (trimmed.Length == 2 && trimmed[0] == '_')
        {
            trimmed = trimmed[1].ToString();
        }

        if (trimmed.Length != 1)
        {
            return false;
        }

        return TryParseLetter(trimmed[0], out supplyPoint);
    }

    internal static bool TryParseLetter(char letter, out GridSupplyPoint supplyPoint)
    {
        supplyPoint = letter switch
        {
            'A' => GridSupplyPoint.A,
            'B' => GridSupplyPoint.B,
            'C' => GridSupplyPoint.C,
            'D' => GridSupplyPoint.D,
            'E' => GridSupplyPoint.E,
            'F' => GridSupplyPoint.F,
            'G' => GridSupplyPoint.G,
            'H' => GridSupplyPoint.H,
            'J' => GridSupplyPoint.J,
            'K' => GridSupplyPoint.K,
            'L' => GridSupplyPoint.L,
            'M' => GridSupplyPoint.M,
            'N' => GridSupplyPoint.N,
            'P' => GridSupplyPoint.P,
            _ => default,
        };

        return letter is 'A' or 'B' or 'C' or 'D' or 'E' or 'F' or 'G' or 'H' or 'J' or 'K' or 'L' or 'M' or 'N' or 'P';
    }
}
