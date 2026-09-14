using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyClientTests
{
    [Fact]
    public void Constructor_Default_CreatesInstance()
    {
        using OctopusEnergyClient client = new();

        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithHttpClient_UsesSuppliedClient()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient client = new(httpClient);

        Assert.NotNull(client);
    }
}
