using Microsoft.Extensions.DependencyInjection;
using OctopusEnergy.Client.DependencyInjection;

namespace OctopusEnergy.Client.Tests.DependencyInjection;

public sealed class OctopusEnergyClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddOctopusEnergyClient_RegistersClient()
    {
        ServiceCollection services = new();
        services.AddOctopusEnergyClient(options =>
        {
            options.ApiKey = "probe-key";
            options.BaseAddress = new Uri(OctopusEnergyClient.DefaultBaseUrl);
        });

        using ServiceProvider provider = services.BuildServiceProvider();
        IOctopusEnergyClient client = provider.GetRequiredService<IOctopusEnergyClient>();

        Assert.NotNull(client);
        Assert.NotNull(client.Products);

        OctopusEnergyClient concrete = Assert.IsType<OctopusEnergyClient>(client);
        Assert.NotNull(concrete.GraphQl);
    }
}
