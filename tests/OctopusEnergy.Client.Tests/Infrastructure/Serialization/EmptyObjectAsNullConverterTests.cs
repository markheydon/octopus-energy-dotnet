using System.Text.Json;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Tests.Infrastructure.Serialization;

public sealed class EmptyObjectAsNullConverterTests
{
    [Fact]
    public void Deserialize_WhenEmptyObject_ReturnsNull()
    {
        ProductPaymentMethodTariffs? tariffs = JsonSerializer.Deserialize<ProductPaymentMethodTariffs>(
            """{"direct_debit_monthly":{}}""",
            OctopusJsonSerializerOptions.Default);

        Assert.NotNull(tariffs);
        Assert.Null(tariffs!.DirectDebitMonthly);
    }

    [Fact]
    public void Deserialize_WhenPopulatedObject_ReturnsTariff()
    {
        ProductPaymentMethodTariffs? tariffs = JsonSerializer.Deserialize<ProductPaymentMethodTariffs>(
            """{"direct_debit_monthly":{"code":"E-1R-AGILE-FLEX-22-11-25-C"}}""",
            OctopusJsonSerializerOptions.Default);

        Assert.NotNull(tariffs?.DirectDebitMonthly);
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-C", tariffs.DirectDebitMonthly!.Code);
    }

    [Fact]
    public void Deserialize_WhenNestedEmptyObject_ReturnsNullNestedProperty()
    {
        ProductSampleRateQuote? quote = JsonSerializer.Deserialize<ProductSampleRateQuote>(
            """
            {
              "electricity_single_rate": { "annual_cost_inc_vat": 90000 },
              "electricity_dual_rate": {}
            }
            """,
            OctopusJsonSerializerOptions.Default);

        Assert.NotNull(quote?.ElectricitySingleRate);
        Assert.Null(quote.ElectricityDualRate);
    }
}
