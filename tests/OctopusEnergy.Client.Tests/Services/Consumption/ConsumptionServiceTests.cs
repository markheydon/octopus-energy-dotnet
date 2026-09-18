using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Consumption;

public sealed class ConsumptionServiceTests
{
    [Fact]
    public async Task ListElectricityAsync_WhenBstFixture_PreservesOffsets()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-bst-spring-forward.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<ConsumptionInterval> intervals = await CollectAsync(
            client.Consumption.ListElectricityAsync("1000000000001", "1111111111", cancellationToken: CancellationToken.None));

        Assert.Equal(4, intervals.Count);
        Assert.Equal(TimeSpan.FromHours(1), intervals[2].IntervalStart.Offset);
        Assert.Equal("/v1/electricity-meter-points/1000000000001/meters/1111111111/consumption/", handler.SentRequests[0].RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenEmptyList_ReturnsNoIntervals()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-empty.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<ConsumptionInterval> intervals = await CollectAsync(
            client.Consumption.ListElectricityAsync("1000000000001", "1111111111", cancellationToken: CancellationToken.None));

        Assert.Empty(intervals);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenFiltersSet_AppendsQueryParameters()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-empty.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ConsumptionListRequest request = new()
        {
            PeriodFrom = new DateTimeOffset(2024, 3, 31, 0, 0, 0, TimeSpan.Zero),
            PeriodTo = new DateTimeOffset(2024, 4, 1, 0, 0, 0, TimeSpan.Zero),
            PageSize = 500,
            OrderBy = ConsumptionOrderBy.PeriodAscending,
            GroupBy = ConsumptionGroupBy.Day,
        };

        await CollectAsync(client.Consumption.ListElectricityAsync("1000000000001", "1111111111", request, CancellationToken.None));

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.Contains("period_from=2024-03-31T00%3A00%3A00Z", query, StringComparison.Ordinal);
        Assert.Contains("period_to=2024-04-01T00%3A00%3A00Z", query, StringComparison.Ordinal);
        Assert.Contains("page_size=500", query, StringComparison.Ordinal);
        Assert.Contains("order_by=period", query, StringComparison.Ordinal);
        Assert.Contains("group_by=day", query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenPageSizeExceedsMaximum_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ConsumptionListRequest request = new() { PageSize = RestPageSizeLimits.ConsumptionMaximum + 1 };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync(
                "1000000000001",
                "1111111111",
                request,
                CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListGasAsync_WhenSmets2_ReturnsCubicMetresUnit()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-gas-smets2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<ConsumptionInterval> intervals = await CollectAsync(
            client.Consumption.ListGasAsync("1234567890", "G1234567", cancellationToken: CancellationToken.None));

        Assert.Single(intervals);
        Assert.Equal(GasConsumptionUnit.CubicMetres, intervals[0].GasUnit);
        Assert.EndsWith("/gas-meter-points/1234567890/meters/G1234567/consumption/", handler.SentRequests[0].RequestUri?.AbsolutePath, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenMpanNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync(null!, "1111111111", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
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
