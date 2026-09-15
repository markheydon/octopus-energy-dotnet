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

    [Fact]
    public void Constructor_WithHttpClientWithoutBaseAddress_SetsDefaultBaseAddress()
    {
        using HttpClient httpClient = new();
        using OctopusEnergyClient client = new(httpClient);

        Assert.Equal(new Uri(OctopusEnergyClient.DefaultBaseUrl), httpClient.BaseAddress);
    }

    [Fact]
    public void Constructor_WithHttpClientWithoutAcceptHeader_AddsJsonAcceptHeader()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };

        using OctopusEnergyClient client = new(httpClient);

        Assert.Contains(
            httpClient.DefaultRequestHeaders.Accept,
            mediaType => string.Equals(mediaType.MediaType, "application/json", StringComparison.OrdinalIgnoreCase));
    }
}
