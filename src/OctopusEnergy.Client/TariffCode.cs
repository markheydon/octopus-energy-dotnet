namespace OctopusEnergy.Client;

/// <summary>
/// Parsed Octopus tariff code (for example <c>E-1R-AGILE-FLEX-22-11-25-C</c>).
/// </summary>
public readonly record struct TariffCode
{
    /// <summary>
    /// Creates a tariff code from its components.
    /// </summary>
    /// <param name="fuel">Electricity or gas.</param>
    /// <param name="registerKind">Single or dual register.</param>
    /// <param name="productCode">The product code segment (may contain hyphens).</param>
    /// <param name="gridSupplyPoint">The UK distribution region suffix.</param>
    public TariffCode(
        EnergyFuel fuel,
        TariffRegisterKind registerKind,
        string productCode,
        GridSupplyPoint gridSupplyPoint)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productCode);

        Fuel = fuel;
        RegisterKind = registerKind;
        ProductCode = productCode;
        GridSupplyPoint = gridSupplyPoint;
    }

    /// <summary>
    /// Electricity or gas.
    /// </summary>
    public EnergyFuel Fuel { get; }

    /// <summary>
    /// Single or dual register count.
    /// </summary>
    public TariffRegisterKind RegisterKind { get; }

    /// <summary>
    /// Product code segment (may contain hyphens).
    /// </summary>
    public string ProductCode { get; }

    /// <summary>
    /// UK distribution region encoded as the final tariff-code letter.
    /// </summary>
    public GridSupplyPoint GridSupplyPoint { get; }

    /// <summary>
    /// Parses a tariff code string.
    /// </summary>
    /// <param name="value">The tariff code (for example <c>E-1R-AGILE-FLEX-22-11-25-C</c>).</param>
    /// <returns>The parsed tariff code.</returns>
    /// <exception cref="OctopusEnergyRequestException">The value is not a valid tariff code.</exception>
    public static TariffCode Parse(string value)
    {
        if (!TryParse(value, out TariffCode tariffCode))
        {
            throw new OctopusEnergyRequestException(
                $"The value '{value}' is not a valid tariff code. Expected the form E-1R-PRODUCT-GSP or G-1R-PRODUCT-GSP.");
        }

        return tariffCode;
    }

    /// <summary>
    /// Attempts to parse a tariff code string.
    /// </summary>
    /// <param name="value">The tariff code to parse.</param>
    /// <param name="tariffCode">The parsed tariff code when successful.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(string? value, out TariffCode tariffCode)
    {
        tariffCode = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        string[] segments = value.Split('-');
        if (segments.Length < 4)
        {
            return false;
        }

        if (!TryParseFuel(segments[0], out EnergyFuel fuel))
        {
            return false;
        }

        if (!TryParseRegisterKind(segments[1], out TariffRegisterKind registerKind))
        {
            return false;
        }

        string gspSegment = segments[^1];
        if (gspSegment.Length != 1)
        {
            return false;
        }

        if (!GridSupplyPointParser.TryParseLetter(gspSegment[0], out GridSupplyPoint gridSupplyPoint))
        {
            return false;
        }

        string productCode = string.Join('-', segments[2..^1]);
        if (string.IsNullOrWhiteSpace(productCode))
        {
            return false;
        }

        tariffCode = new TariffCode(fuel, registerKind, productCode, gridSupplyPoint);
        return true;
    }

    /// <summary>
    /// Returns the wire-format tariff code string.
    /// </summary>
    /// <returns>The tariff code (for example <c>E-1R-AGILE-FLEX-22-11-25-C</c>).</returns>
    public override string ToString()
    {
        char gspLetter = GridSupplyPointParser.ToLetter(GridSupplyPoint);
        return $"{ToFuelPrefix(Fuel)}-{ToRegisterPrefix(RegisterKind)}-{ProductCode}-{gspLetter}";
    }

    /// <summary>
    /// Builds the relative REST path for a tariff charge list endpoint.
    /// </summary>
    /// <param name="chargeKind">The charge list to request.</param>
    /// <returns>
    /// A relative path suitable for <see cref="Infrastructure.Http.RestClient"/> (for example
    /// <c>products/AGILE-FLEX-22-11-25/electricity-tariffs/E-1R-AGILE-FLEX-22-11-25-C/standard-unit-rates/</c>).
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Day or night unit-rate paths are not available for gas tariffs or single-register electricity tariffs.
    /// </exception>
    [Obsolete("Use TariffRatesService to list standing charges and unit rates instead of building REST paths manually.")]
    public string GetRelativeChargePath(TariffChargeKind chargeKind)
    {
        return BuildRelativeChargePath(chargeKind);
    }

    /// <summary>
    /// Builds the relative REST path for a tariff charge list endpoint.
    /// </summary>
    /// <param name="chargeKind">The charge list to request.</param>
    /// <returns>
    /// A relative path suitable for <see cref="Infrastructure.Http.RestClient"/> (for example
    /// <c>products/AGILE-FLEX-22-11-25/electricity-tariffs/E-1R-AGILE-FLEX-22-11-25-C/standard-unit-rates/</c>).
    /// </returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Day or night unit-rate paths are not available for gas tariffs or single-register electricity tariffs.
    /// </exception>
    internal string BuildRelativeChargePath(TariffChargeKind chargeKind)
    {
        if (chargeKind is TariffChargeKind.DayUnitRates or TariffChargeKind.NightUnitRates)
        {
            if (Fuel == EnergyFuel.Gas)
            {
                throw new OctopusEnergyRequestException(
                    "Day and night unit-rate paths are not available for gas tariffs.");
            }

            if (RegisterKind != TariffRegisterKind.DualRegister)
            {
                throw new OctopusEnergyRequestException(
                    "Day and night unit-rate paths apply to dual-register electricity tariffs only.");
            }
        }

        string tariffSegment = Fuel == EnergyFuel.Electricity ? "electricity-tariffs" : "gas-tariffs";
        string chargeSegment = ToChargeSegment(chargeKind);

        return $"products/{ProductCode}/{tariffSegment}/{ToString()}/{chargeSegment}/";
    }

    private static bool TryParseFuel(string segment, out EnergyFuel fuel)
    {
        fuel = segment switch
        {
            "E" => EnergyFuel.Electricity,
            "G" => EnergyFuel.Gas,
            _ => default,
        };

        return segment is "E" or "G";
    }

    private static bool TryParseRegisterKind(string segment, out TariffRegisterKind registerKind)
    {
        registerKind = segment switch
        {
            "1R" => TariffRegisterKind.SingleRegister,
            "2R" => TariffRegisterKind.DualRegister,
            _ => default,
        };

        return segment is "1R" or "2R";
    }

    private static string ToFuelPrefix(EnergyFuel fuel)
    {
        return fuel switch
        {
            EnergyFuel.Electricity => "E",
            EnergyFuel.Gas => "G",
            _ => throw new ArgumentOutOfRangeException(nameof(fuel), fuel, "Unknown fuel."),
        };
    }

    private static string ToRegisterPrefix(TariffRegisterKind registerKind)
    {
        return registerKind switch
        {
            TariffRegisterKind.SingleRegister => "1R",
            TariffRegisterKind.DualRegister => "2R",
            _ => throw new ArgumentOutOfRangeException(nameof(registerKind), registerKind, "Unknown register kind."),
        };
    }

    private static string ToChargeSegment(TariffChargeKind chargeKind)
    {
        return chargeKind switch
        {
            TariffChargeKind.StandingCharges => "standing-charges",
            TariffChargeKind.StandardUnitRates => "standard-unit-rates",
            TariffChargeKind.DayUnitRates => "day-unit-rates",
            TariffChargeKind.NightUnitRates => "night-unit-rates",
            _ => throw new ArgumentOutOfRangeException(nameof(chargeKind), chargeKind, "Unknown charge kind."),
        };
    }
}
