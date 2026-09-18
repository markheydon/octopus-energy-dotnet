using OctopusEnergy.Client;

namespace OctopusEnergy.Client.Tests;

public sealed class GridSupplyPointTests
{
    [Theory]
    [InlineData("_C", GridSupplyPoint.C)]
    [InlineData("C", GridSupplyPoint.C)]
    [InlineData("_A", GridSupplyPoint.A)]
    [InlineData("P", GridSupplyPoint.P)]
    public void Parse_WhenValidGroupIdOrLetter_ReturnsSupplyPoint(string value, GridSupplyPoint expected)
    {
        GridSupplyPoint actual = GridSupplyPointParser.Parse(value);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToGroupId_WhenLondon_ReturnsUnderscoreC()
    {
        Assert.Equal("_C", GridSupplyPointParser.ToGroupId(GridSupplyPoint.C));
    }

    [Fact]
    public void ToLetter_WhenLondon_ReturnsC()
    {
        Assert.Equal('C', GridSupplyPointParser.ToLetter(GridSupplyPoint.C));
    }

    [Theory]
    [InlineData(GridSupplyPoint.A, 'A')]
    [InlineData(GridSupplyPoint.B, 'B')]
    [InlineData(GridSupplyPoint.C, 'C')]
    [InlineData(GridSupplyPoint.D, 'D')]
    [InlineData(GridSupplyPoint.E, 'E')]
    [InlineData(GridSupplyPoint.F, 'F')]
    [InlineData(GridSupplyPoint.G, 'G')]
    [InlineData(GridSupplyPoint.H, 'H')]
    [InlineData(GridSupplyPoint.J, 'J')]
    [InlineData(GridSupplyPoint.K, 'K')]
    [InlineData(GridSupplyPoint.L, 'L')]
    [InlineData(GridSupplyPoint.M, 'M')]
    [InlineData(GridSupplyPoint.N, 'N')]
    [InlineData(GridSupplyPoint.P, 'P')]
    public void ToLetter_WhenValidSupplyPoint_ReturnsExpectedLetter(GridSupplyPoint supplyPoint, char expected)
    {
        Assert.Equal(expected, GridSupplyPointParser.ToLetter(supplyPoint));
    }

    [Theory]
    [InlineData("I")]
    [InlineData("_I")]
    [InlineData("")]
    [InlineData("London")]
    public void Parse_WhenInvalidValue_ThrowsOctopusEnergyRequestException(string value)
    {
        OctopusEnergyRequestException exception = Assert.Throws<OctopusEnergyRequestException>(
            () => GridSupplyPointParser.Parse(value));

        Assert.Contains("grid supply point", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("I")]
    [InlineData("_I")]
    [InlineData("")]
    [InlineData("London")]
    public void TryParse_WhenInvalidValue_ReturnsFalse(string value)
    {
        Assert.False(GridSupplyPointParser.TryParse(value, out GridSupplyPoint _));
    }
}
