using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
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
    public async Task ListElectricityPageAsync_WhenFixture_ReturnsCountAndIntervals()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-bst-spring-forward.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResult<ConsumptionInterval> page = await client.Consumption.ListElectricityPageAsync(
            "1000000000001",
            "1111111111",
            cancellationToken: CancellationToken.None);

        Assert.Equal(4, page.Count);
        Assert.Equal(4, page.Results.Count);
        Assert.Null(page.Next);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityPageAsync_WhenNextPageAvailable_DoesNotFollowNext()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResult<ConsumptionInterval> page = await client.Consumption.ListElectricityPageAsync(
            "1000000000001",
            "1111111111",
            cancellationToken: CancellationToken.None);

        Assert.Equal(3, page.Count);
        Assert.Equal(2, page.Results.Count);
        Assert.NotNull(page.Next);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenTwoPages_ReturnsAllIntervals()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<ConsumptionInterval> intervals = await CollectAsync(
            client.Consumption.ListElectricityAsync("1000000000001", "1111111111", cancellationToken: CancellationToken.None));

        Assert.Equal(3, intervals.Count);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenPageSizeIsZero_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ConsumptionListRequest request = new() { PageSize = 0 };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync(
                "1000000000001",
                "1111111111",
                request,
                CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityPageAsync_WhenPageSizeIsZero_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ConsumptionListRequest request = new() { PageSize = 0 };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Consumption.ListElectricityPageAsync(
                "1000000000001",
                "1111111111",
                request,
                CancellationToken.None));

        Assert.Empty(handler.SentRequests);
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
    public async Task ListGasPageAsync_WhenFixture_ReturnsCountAndIntervals()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-gas-smets2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResult<ConsumptionInterval> page = await client.Consumption.ListGasPageAsync(
            "1234567890",
            "G1234567",
            cancellationToken: CancellationToken.None);

        Assert.Equal(1, page.Count);
        Assert.Single(page.Results);
        Assert.Equal(GasConsumptionUnit.CubicMetres, page.Results[0].GasUnit);
        Assert.Null(page.Next);
        Assert.Single(handler.SentRequests);
    }

    [Fact]
    public async Task ListGasPageAsync_WhenNextPageAvailable_DoesNotFollowNext()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-pagination-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        PaginatedResult<ConsumptionInterval> page = await client.Consumption.ListGasPageAsync(
            "1234567890",
            "G1234567",
            cancellationToken: CancellationToken.None);

        Assert.Equal(3, page.Count);
        Assert.Equal(2, page.Results.Count);
        Assert.NotNull(page.Next);
        Assert.Single(handler.SentRequests);
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
    public async Task ListElectricityAsync_WhenMeterPointProvided_UsesMpanFromModel()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-empty.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ElectricityMeterPoint meterPoint = new()
        {
            Mpan = "1000000000001",
        };

        await CollectAsync(
            client.Consumption.ListElectricityAsync(meterPoint, "1111111111", cancellationToken: CancellationToken.None));

        Assert.EndsWith(
            "/electricity-meter-points/1000000000001/meters/1111111111/consumption/",
            handler.SentRequests[0].RequestUri?.AbsolutePath,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListGasAsync_WhenMeterPointProvided_UsesMprnFromModel()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("consumption-gas-smets2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        GasMeterPoint meterPoint = new()
        {
            Mprn = "1234567890",
        };

        await CollectAsync(
            client.Consumption.ListGasAsync(meterPoint, "G1234567", cancellationToken: CancellationToken.None));

        Assert.EndsWith(
            "/gas-meter-points/1234567890/meters/G1234567/consumption/",
            handler.SentRequests[0].RequestUri?.AbsolutePath,
            StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenMeterPointNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync((ElectricityMeterPoint)null!, "1111111111", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenMeterPointMpanMissing_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ElectricityMeterPoint meterPoint = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync(meterPoint, "1111111111", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListGasAsync_WhenMeterPointNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => CollectAsync(client.Consumption.ListGasAsync((GasMeterPoint)null!, "G1234567", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListGasAsync_WhenMeterPointMprnMissing_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        GasMeterPoint meterPoint = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListGasAsync(meterPoint, "G1234567", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenMeterPointAndMeterSerialMissing_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ElectricityMeterPoint meterPoint = new()
        {
            Mpan = "1000000000001",
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync(meterPoint, "   ", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListGasAsync_WhenMeterPointAndMeterSerialMissing_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        GasMeterPoint meterPoint = new()
        {
            Mprn = "1234567890",
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListGasAsync(meterPoint, "   ", cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListElectricityAsync_WhenMpanNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Consumption.ListElectricityAsync((string)null!, "1111111111", cancellationToken: CancellationToken.None)));

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
