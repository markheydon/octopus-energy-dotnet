using System.Text.Json;
using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class GridSupplyPointJsonConverterTests
{
    [Fact]
    public void Deserialize_WhenGroupId_ReadsSupplyPoint()
    {
        GridSupplyPoint supplyPoint = JsonSerializer.Deserialize<GridSupplyPoint>("\"_C\"");

        Assert.Equal(GridSupplyPoint.C, supplyPoint);
    }

    [Fact]
    public void Serialize_WhenSupplyPoint_WritesGroupId()
    {
        string json = JsonSerializer.Serialize(GridSupplyPoint.C);

        Assert.Equal("\"_C\"", json);
    }
}
