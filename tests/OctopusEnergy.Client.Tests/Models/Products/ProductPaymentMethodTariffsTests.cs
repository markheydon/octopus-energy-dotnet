using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Tests.Models.Products;

public sealed class ProductPaymentMethodTariffsTests
{
    [Fact]
    public void GetTariff_WhenDirectDebitMonthly_ReturnsMonthlyTariff()
    {
        ProductTariff monthly = new() { Code = "E-1R-TEST-A" };
        ProductPaymentMethodTariffs tariffs = new()
        {
            DirectDebitMonthly = monthly,
        };

        Assert.Same(monthly, tariffs.GetTariff(ProductPaymentMethod.DirectDebitMonthly));
        Assert.Null(tariffs.GetTariff(ProductPaymentMethod.DirectDebitQuarterly));
    }
}
