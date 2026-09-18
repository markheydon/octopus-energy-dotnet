using System.Text.Json;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Tests.Infrastructure.Serialization;

public sealed class GridSupplyPointDictionaryConverterTests
{
    [Fact]
    public void Deserialize_WhenNull_ReturnsEmptyDictionary()
    {
        ProductDetail detail = Deserialize("""{"code":"X","single_register_electricity_tariffs":null}""");

        Assert.NotNull(detail.SingleRegisterElectricityTariffs);
        Assert.Empty(detail.SingleRegisterElectricityTariffs);
    }

    [Fact]
    public void Deserialize_WhenEmptyObject_ReturnsEmptyDictionary()
    {
        ProductDetail detail = Deserialize("""{"code":"X","single_register_electricity_tariffs":{}}""");

        Assert.NotNull(detail.SingleRegisterElectricityTariffs);
        Assert.Empty(detail.SingleRegisterElectricityTariffs);
    }

    [Fact]
    public void Deserialize_WhenUnknownGspKey_SkipsKeyAndKeepsKnownRegions()
    {
        ProductDetail detail = Deserialize(
            """
            {
              "code": "X",
              "single_register_electricity_tariffs": {
                "_Q": { "direct_debit_monthly": { "code": "UNKNOWN" } },
                "_C": { "direct_debit_monthly": { "code": "E-1R-AGILE-FLEX-22-11-25-C" } }
              }
            }
            """);

        Assert.Single(detail.SingleRegisterElectricityTariffs);
        Assert.True(detail.SingleRegisterElectricityTariffs.ContainsKey(GridSupplyPoint.C));
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-C", detail.SingleRegisterElectricityTariffs[GridSupplyPoint.C].DirectDebitMonthly!.Code);
    }

    private static ProductDetail Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ProductDetail>(json, OctopusJsonSerializerOptions.Default)
            ?? throw new InvalidOperationException("Expected product detail JSON to deserialise.");
    }
}
