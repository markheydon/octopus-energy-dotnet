using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;

namespace OctopusEnergy.Client.Tests.Models.Accounts;

public sealed class TariffAgreementTests
{
    [Fact]
    public void ParsedTariffCode_WhenValidWireCode_ReturnsParsedValue()
    {
        TariffAgreement agreement = new()
        {
            TariffCode = "E-1R-VAR-22-11-01-N",
        };

        TariffCode? parsed = agreement.ParsedTariffCode;

        Assert.NotNull(parsed);
        Assert.Equal(EnergyFuel.Electricity, parsed.Value.Fuel);
        Assert.Equal(TariffRegisterKind.SingleRegister, parsed.Value.RegisterKind);
        Assert.Equal(GridSupplyPoint.N, parsed.Value.GridSupplyPoint);
        Assert.Equal("E-1R-VAR-22-11-01-N", parsed.Value.ToString());
    }

    [Fact]
    public void ParsedTariffCode_WhenInvalidWireCode_ReturnsNull()
    {
        TariffAgreement agreement = new()
        {
            TariffCode = "NOT-A-TARIFF-CODE",
        };

        Assert.Null(agreement.ParsedTariffCode);
    }
}
