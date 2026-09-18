using System.Text;
using OctopusEnergy.Client.Infrastructure.Authentication;

namespace OctopusEnergy.Client.Tests.Infrastructure.Authentication;

public sealed class BasicApiKeyHeaderTests
{
    [Fact]
    public void Create_WhenValidKey_EncodesKeyWithTrailingColon()
    {
        const string apiKey = "test-api-key-value";

        System.Net.Http.Headers.AuthenticationHeaderValue header = BasicApiKeyHeader.Create(apiKey);

        Assert.Equal("Basic", header.Scheme);
        Assert.Equal(
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{apiKey}:")),
            header.Parameter);
    }

    [Fact]
    public void Create_WhenValidKey_DoesNotMatchEncodingWithoutColon()
    {
        const string apiKey = "test-api-key-value";

        System.Net.Http.Headers.AuthenticationHeaderValue header = BasicApiKeyHeader.Create(apiKey);
        string withoutColon = Convert.ToBase64String(Encoding.UTF8.GetBytes(apiKey));

        Assert.NotEqual(withoutColon, header.Parameter);
    }

    [Fact]
    public void Create_WhenNull_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => BasicApiKeyHeader.Create(null!));

        Assert.Equal("apiKey", exception.ParamName);
    }

    [Fact]
    public void Create_WhenWhitespace_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(() => BasicApiKeyHeader.Create("   "));

        Assert.Equal("apiKey", exception.ParamName);
    }
}
