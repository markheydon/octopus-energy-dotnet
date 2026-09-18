using OctopusEnergy.Client.Infrastructure.Http;

namespace OctopusEnergy.Client.Tests.Infrastructure.Http;

public sealed class RestQueryTests
{
    [Fact]
    public void Append_WhenNoParameters_ReturnsRelativePath()
    {
        string path = RestQuery.Append("products/", []);

        Assert.Equal("products/", path);
    }

    [Fact]
    public void Append_WhenParametersSet_BuildsQueryString()
    {
        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter("brand", "OCTOPUS_ENERGY"),
            new RestQuery.QueryParameter("is_green", "true"),
        ];

        string path = RestQuery.Append("products/", parameters);

        Assert.Equal("products/?brand=OCTOPUS_ENERGY&is_green=true", path);
    }

    [Fact]
    public void Append_WhenPathAlreadyHasQuery_AppendsWithAmpersand()
    {
        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter("page", "2"),
        ];

        string path = RestQuery.Append("products/?brand=OCTOPUS_ENERGY", parameters);

        Assert.Equal("products/?brand=OCTOPUS_ENERGY&page=2", path);
    }

    [Fact]
    public void Append_WhenParameterValueNull_OmitsParameter()
    {
        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter("brand", null),
            new RestQuery.QueryParameter("is_green", "true"),
        ];

        string path = RestQuery.Append("products/", parameters);

        Assert.Equal("products/?is_green=true", path);
    }

    [Fact]
    public void Append_WhenParameterValueNeedsEncoding_EscapesValue()
    {
        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter("brand", "A&B"),
        ];

        string path = RestQuery.Append("products/", parameters);

        Assert.Equal("products/?brand=A%26B", path);
    }

    [Fact]
    public void FormatDateTimeOffset_WhenWholeSeconds_FormatsWithoutFraction()
    {
        DateTimeOffset value = new(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2019-01-01T00:00:00Z", formatted);
    }

    [Fact]
    public void FormatDateTimeOffset_WhenFractionalSeconds_PreservesPrecision()
    {
        DateTimeOffset value = new DateTimeOffset(2023, 11, 10, 0, 21, 44, 970, TimeSpan.Zero).AddTicks(7110);

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2023-11-10T00:21:44.970711Z", formatted);
    }

    [Fact]
    public void FormatDateTimeOffset_WhenLocalOffset_ConvertsToUtc()
    {
        DateTimeOffset value = new(2019, 1, 1, 1, 0, 0, TimeSpan.FromHours(1));

        string formatted = RestQuery.FormatDateTimeOffset(value);

        Assert.Equal("2019-01-01T00:00:00Z", formatted);
    }
}
