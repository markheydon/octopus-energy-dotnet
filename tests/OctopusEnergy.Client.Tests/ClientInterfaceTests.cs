using OctopusEnergy.Client;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.Products;

namespace OctopusEnergy.Client.Tests;

public sealed class ClientInterfaceTests
{
    [Fact]
    public void OctopusEnergyClient_ImplementsIOctopusEnergyClient()
    {
        using OctopusEnergyClient client = new();

        Assert.IsAssignableFrom<IOctopusEnergyClient>(client);
        Assert.IsAssignableFrom<IAccountService>(client.Accounts);
        Assert.IsAssignableFrom<IConsumptionService>(client.Consumption);
        Assert.IsAssignableFrom<IIndustryService>(client.Industry);
        Assert.IsAssignableFrom<IProductService>(client.Products);
        Assert.IsAssignableFrom<ITariffRatesService>(client.TariffRates);
    }

    [Fact]
    public void ResourceServices_ExposeConcreteImplementations()
    {
        using OctopusEnergyClient client = new();

        Assert.IsType<AccountService>(client.Accounts);
        Assert.IsType<ConsumptionService>(client.Consumption);
        Assert.IsType<IndustryService>(client.Industry);
        Assert.IsType<ProductService>(client.Products);
        Assert.IsType<TariffRatesService>(client.TariffRates);
    }
}
