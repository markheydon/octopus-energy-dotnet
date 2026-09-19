using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class TariffCodeTests
{
    private const string AgileElectricity = "E-1R-AGILE-FLEX-22-11-25-C";
    private const string EconomySevenElectricity = "E-2R-VAR-22-11-01-A";
    private const string GasTariff = "G-1R-VAR-22-11-01-N";

    [Theory]
    [InlineData(AgileElectricity)]
    [InlineData(EconomySevenElectricity)]
    [InlineData(GasTariff)]
    public void Parse_WhenValidCode_RoundTrips(string value)
    {
        TariffCode tariffCode = TariffCode.Parse(value);

        Assert.Equal(value, tariffCode.ToString());
    }

    [Fact]
    public void Constructor_WhenComponentsProvided_FormatsCode()
    {
        TariffCode tariffCode = new(
            EnergyFuel.Electricity,
            TariffRegisterKind.SingleRegister,
            "AGILE-FLEX-22-11-25",
            GridSupplyPoint.C);

        Assert.Equal(AgileElectricity, tariffCode.ToString());
        Assert.Equal(EnergyFuel.Electricity, tariffCode.Fuel);
        Assert.Equal(TariffRegisterKind.SingleRegister, tariffCode.RegisterKind);
        Assert.Equal("AGILE-FLEX-22-11-25", tariffCode.ProductCode);
        Assert.Equal(GridSupplyPoint.C, tariffCode.GridSupplyPoint);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenElectricityStandard_ReturnsExpectedPath()
    {
        TariffCode tariffCode = TariffCode.Parse(AgileElectricity);

        string path = tariffCode.BuildRelativeChargePath(TariffChargeKind.StandardUnitRates);

        Assert.Equal(
            "products/AGILE-FLEX-22-11-25/electricity-tariffs/E-1R-AGILE-FLEX-22-11-25-C/standard-unit-rates/",
            path);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenElectricityDay_ReturnsDayUnitRatesPath()
    {
        TariffCode tariffCode = TariffCode.Parse(EconomySevenElectricity);

        string path = tariffCode.BuildRelativeChargePath(TariffChargeKind.DayUnitRates);

        Assert.Equal(
            "products/VAR-22-11-01/electricity-tariffs/E-2R-VAR-22-11-01-A/day-unit-rates/",
            path);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenElectricityNight_ReturnsNightUnitRatesPath()
    {
        TariffCode tariffCode = TariffCode.Parse(EconomySevenElectricity);

        string path = tariffCode.BuildRelativeChargePath(TariffChargeKind.NightUnitRates);

        Assert.Equal(
            "products/VAR-22-11-01/electricity-tariffs/E-2R-VAR-22-11-01-A/night-unit-rates/",
            path);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenGasStanding_ReturnsGasTariffPath()
    {
        TariffCode tariffCode = TariffCode.Parse(GasTariff);

        string path = tariffCode.BuildRelativeChargePath(TariffChargeKind.StandingCharges);

        Assert.Equal(
            "products/VAR-22-11-01/gas-tariffs/G-1R-VAR-22-11-01-N/standing-charges/",
            path);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenGasDayUnitRates_ThrowsOctopusEnergyRequestException()
    {
        TariffCode tariffCode = TariffCode.Parse(GasTariff);

        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => tariffCode.BuildRelativeChargePath(TariffChargeKind.DayUnitRates));

        Assert.Contains("gas", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildRelativeChargePath_WhenSingleRegisterElectricityDayUnitRates_ThrowsOctopusEnergyRequestException()
    {
        TariffCode tariffCode = TariffCode.Parse(AgileElectricity);

        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => tariffCode.BuildRelativeChargePath(TariffChargeKind.DayUnitRates));

        Assert.Contains("dual-register", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryParse_WhenValidCode_ReturnsTrue()
    {
        Assert.True(TariffCode.TryParse(AgileElectricity, out TariffCode tariffCode));
        Assert.Equal(AgileElectricity, tariffCode.ToString());
    }

    [Theory]
    [InlineData("E-1R-PRODUCT-")]
    [InlineData("E-1R-PRODUCT-C-")]
    public void TryParse_WhenTrailingHyphen_ReturnsFalse(string value)
    {
        Assert.False(TariffCode.TryParse(value, out TariffCode _));
    }

    [Theory]
    [InlineData("X-1R-PRODUCT-C")]
    [InlineData("E-3R-PRODUCT-C")]
    [InlineData("E-1R-PRODUCT-I")]
    [InlineData("E-1R")]
    [InlineData("")]
    public void Parse_WhenInvalidCode_ThrowsOctopusEnergyRequestException(string value)
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => TariffCode.Parse(value));

        Assert.Contains("tariff code", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("X-1R-PRODUCT-C")]
    [InlineData("E-3R-PRODUCT-C")]
    [InlineData("E-1R-PRODUCT-I")]
    public void TryParse_WhenInvalidCode_ReturnsFalse(string value)
    {
        Assert.False(TariffCode.TryParse(value, out TariffCode _));
    }
}
