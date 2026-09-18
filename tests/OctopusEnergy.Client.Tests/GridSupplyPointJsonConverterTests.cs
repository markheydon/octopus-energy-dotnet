using System.Text.Json;
using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Serialization;

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

    [Fact]
    public void Deserialize_WhenModelProperty_ReadsSupplyPoint()
    {
        GridSupplyPointModel? model = JsonSerializer.Deserialize<GridSupplyPointModel>(
            """{"gsp":"_C"}""",
            OctopusJsonSerializerOptions.Default);

        Assert.NotNull(model);
        Assert.Equal(GridSupplyPoint.C, model!.Gsp);
    }

    [Fact]
    public void Serialize_WhenModelProperty_WritesGroupId()
    {
        string json = JsonSerializer.Serialize(
            new GridSupplyPointModel { Gsp = GridSupplyPoint.C },
            OctopusJsonSerializerOptions.Default);

        Assert.Equal("""{"gsp":"_C"}""", json);
    }

    private sealed class GridSupplyPointModel
    {
        [JsonPropertyName("gsp")]
        public GridSupplyPoint Gsp { get; init; }
    }
}
