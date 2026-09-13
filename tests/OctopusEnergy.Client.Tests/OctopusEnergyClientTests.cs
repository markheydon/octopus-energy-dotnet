using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class OctopusEnergyClientTests
{
    [Fact]
    public void Constructor_Default_CreatesInstance()
    {
        OctopusEnergyClient client = new();

        Assert.NotNull(client);
    }
}
