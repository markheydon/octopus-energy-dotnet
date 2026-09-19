using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Tests.Models.Products;

public sealed class ProductTariffTests
{
    [Fact]
    public void ParsedTariffCode_WhenValidWireCode_ReturnsParsedValue()
    {
        ProductTariff tariff = new()
        {
            Code = "E-1R-AGILE-FLEX-22-11-25-C",
        };

        TariffCode? parsed = tariff.ParsedTariffCode;

        Assert.NotNull(parsed);
        Assert.Equal("AGILE-FLEX-22-11-25", parsed.Value.ProductCode);
        Assert.Equal(GridSupplyPoint.C, parsed.Value.GridSupplyPoint);
    }

    [Fact]
    public void ParsedTariffCode_WhenInvalidWireCode_ReturnsNull()
    {
        ProductTariff tariff = new()
        {
            Code = "INVALID",
        };

        Assert.Null(tariff.ParsedTariffCode);
    }
}
