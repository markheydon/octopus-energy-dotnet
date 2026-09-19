using System.Text.Json;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Models.Products;

public sealed class ProductDetailTests
{
    [Fact]
    public void TryGetTariff_WhenRegionAndPaymentMethodExist_ReturnsTariff()
    {
        ProductDetail detail = DeserializeFixture();

        bool found = detail.TryGetTariff(
            EnergyFuel.Electricity,
            TariffRegisterKind.SingleRegister,
            GridSupplyPoint.C,
            ProductPaymentMethod.DirectDebitMonthly,
            out ProductTariff? tariff);

        Assert.True(found);
        Assert.NotNull(tariff);
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-C", tariff.Code);
        Assert.NotNull(tariff.ParsedTariffCode);
        Assert.Equal(GridSupplyPoint.C, tariff.ParsedTariffCode.Value.GridSupplyPoint);
    }

    [Fact]
    public void TryGetTariff_WhenPaymentMethodMissing_ReturnsFalse()
    {
        ProductDetail detail = DeserializeFixture();

        bool found = detail.TryGetTariff(
            EnergyFuel.Electricity,
            TariffRegisterKind.SingleRegister,
            GridSupplyPoint.A,
            ProductPaymentMethod.DirectDebitQuarterly,
            out ProductTariff? tariff);

        Assert.False(found);
        Assert.Null(tariff);
    }

    [Fact]
    public void TryGetTariff_WhenRegionMissing_ReturnsFalse()
    {
        ProductDetail detail = DeserializeFixture();

        bool found = detail.TryGetTariff(
            EnergyFuel.Electricity,
            TariffRegisterKind.SingleRegister,
            GridSupplyPoint.B,
            ProductPaymentMethod.DirectDebitMonthly,
            out ProductTariff? tariff);

        Assert.False(found);
        Assert.Null(tariff);
    }

    [Fact]
    public void TryGetTariff_WhenTariffCodeProvided_ResolvesMatchingTariff()
    {
        ProductDetail detail = DeserializeFixture();
        TariffCode tariffCode = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-A");

        bool found = detail.TryGetTariff(
            tariffCode,
            ProductPaymentMethod.DirectDebitMonthly,
            out ProductTariff? tariff);

        Assert.True(found);
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-A", tariff!.Code);
    }

    [Fact]
    public void TryGetTariff_WhenTariffCodeProductCodeMismatch_ReturnsFalse()
    {
        ProductDetail detail = DeserializeFixture();
        TariffCode tariffCode = TariffCode.Parse("E-1R-VAR-22-11-01-C");

        bool found = detail.TryGetTariff(
            tariffCode,
            ProductPaymentMethod.DirectDebitMonthly,
            out ProductTariff? tariff);

        Assert.False(found);
        Assert.Null(tariff);
    }

    [Fact]
    public void TryGetTariff_WhenDualRegisterGasRequested_ThrowsOctopusEnergyRequestException()
    {
        ProductDetail detail = DeserializeFixture();

        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => detail.TryGetTariff(
                EnergyFuel.Gas,
                TariffRegisterKind.DualRegister,
                GridSupplyPoint.A,
                ProductPaymentMethod.DirectDebitMonthly,
                out ProductTariff? _));

        Assert.Contains("dual-register gas", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    private static ProductDetail DeserializeFixture()
    {
        return JsonSerializer.Deserialize<ProductDetail>(
                FixtureFile.Read("products-agile-detail.json"),
                OctopusJsonSerializerOptions.Default)
            ?? throw new InvalidOperationException("Expected product detail JSON to deserialise.");
    }
}
