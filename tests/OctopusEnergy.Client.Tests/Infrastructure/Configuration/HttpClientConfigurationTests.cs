using OctopusEnergy.Client.Infrastructure.Configuration;

namespace OctopusEnergy.Client.Tests.Infrastructure.Configuration;

public sealed class HttpClientConfigurationTests
{
    [Fact]
    public void NormalizeBaseAddress_WhenMissingTrailingSlash_AppendsSlash()
    {
        Uri normalized = HttpClientConfiguration.NormalizeBaseAddress(
            new Uri("https://api.example.test/v1"));

        Assert.Equal(new Uri("https://api.example.test/v1/"), normalized);
    }

    [Fact]
    public void NormalizeBaseAddress_WhenRelative_ThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            () => HttpClientConfiguration.NormalizeBaseAddress(new Uri("/v1/", UriKind.Relative)));

        Assert.Equal("baseAddress", exception.ParamName);
    }

    [Fact]
    public void ApplyBaseAddress_WhenExistingBaseAddressMissingTrailingSlash_AppendsSlash()
    {
        using HttpClient httpClient = new()
        {
            BaseAddress = new Uri("https://api.example.test/v1"),
        };

        HttpClientConfiguration.ApplyBaseAddress(
            httpClient,
            baseAddress: null,
            defaultBaseUrl: "https://api.octopus.energy/v1/");

        Assert.Equal(new Uri("https://api.example.test/v1/"), httpClient.BaseAddress);
    }

    [Fact]
    public void ApplyBaseAddress_WhenDefaultApplied_NormalizesDefaultUrl()
    {
        using HttpClient httpClient = new();

        HttpClientConfiguration.ApplyBaseAddress(
            httpClient,
            baseAddress: null,
            defaultBaseUrl: "https://api.octopus.energy/v1/");

        Assert.Equal(new Uri("https://api.octopus.energy/v1/"), httpClient.BaseAddress);
    }
}
