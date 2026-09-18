using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Industry;

public sealed class IndustryServiceTests
{
    [Fact]
    public async Task ListGridSupplyPointsByPostcodeAsync_WhenValidPostcode_ReturnsGspRecord()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("industry-grid-supply-points-london.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<GridSupplyPointLookup> results = await CollectAsync(
            client.Industry.ListGridSupplyPointsByPostcodeAsync("W1 1AA", CancellationToken.None));

        Assert.Single(results);
        Assert.Equal(GridSupplyPoint.C, results[0].GridSupplyPoint);
        Assert.Equal(GridSupplyPoint.C, results[0].Gsp);
        Assert.Equal("W1 1AA", results[0].Postcode);
        Assert.Contains("postcode=W1%201AA", handler.SentRequests[0].RequestUri?.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListGridSupplyPointsByPostcodeAsync_WhenPostcodeHasSpaces_TrimsValue()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("industry-grid-supply-points-london.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await CollectAsync(client.Industry.ListGridSupplyPointsByPostcodeAsync("  W1 1AA  ", CancellationToken.None));

        Assert.Contains("postcode=W1%201AA", handler.SentRequests[0].RequestUri?.Query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListGridSupplyPointsByPostcodeAsync_WhenPostcodeNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Industry.ListGridSupplyPointsByPostcodeAsync(null!, CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListGridSupplyPointsByPostcodeAsync_WhenNotFound_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("industry-grid-supply-points-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            async () => await CollectAsync(client.Industry.ListGridSupplyPointsByPostcodeAsync("INVALID", CancellationToken.None)));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    [Fact]
    public async Task GetElectricityMeterPointAsync_WhenValidMpan_ReturnsGspAndProfileClass()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("electricity-meter-point-london.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ElectricityMeterPointLookup lookup = await client.Industry.GetElectricityMeterPointAsync(
            "1000000000001",
            CancellationToken.None);

        Assert.Equal(GridSupplyPoint.C, lookup.GridSupplyPoint);
        Assert.Equal(1, lookup.ProfileClass);
        Assert.Equal("/v1/electricity-meter-points/1000000000001/", handler.SentRequests[0].RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task GetElectricityMeterPointAsync_WhenMpanNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Industry.GetElectricityMeterPointAsync(null!, CancellationToken.None));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task GetElectricityMeterPointAsync_WhenNotFound_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("electricity-meter-point-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Industry.GetElectricityMeterPointAsync("0000000000000", CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
    }

    private static HttpClient CreateHttpClient(QueuedHttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
    }

    private static async Task<List<T>> CollectAsync<T>(IAsyncEnumerable<T> source)
    {
        List<T> items = [];
        await foreach (T item in source)
        {
            items.Add(item);
        }

        return items;
    }
}
