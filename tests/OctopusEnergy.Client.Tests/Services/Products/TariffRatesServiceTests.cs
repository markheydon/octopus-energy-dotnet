using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Products;

public sealed class TariffRatesServiceTests
{
    private const string AgileTariff = "E-1R-AGILE-FLEX-22-11-25-C";
    private const string EconomySevenTariff = "E-2R-VAR-22-11-01-A";
    private const string GasTariff = "G-1R-VAR-22-11-01-N";

    [Fact]
    public async Task ListStandardUnitRatesAsync_WhenTwoPages_ReturnsAllRates()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-agile-half-hours.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-agile-half-hours-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TariffCharge> rates = await CollectAsync(
            client.TariffRates.ListStandardUnitRatesAsync(TariffCode.Parse(AgileTariff), cancellationToken: CancellationToken.None));

        Assert.Equal(3, rates.Count);
        Assert.Equal(18.3015m, rates[0].ValueIncVat);
        Assert.Equal(new DateTimeOffset(2024, 3, 31, 1, 0, 0, TimeSpan.Zero), rates[2].ValidFrom);
    }

    [Fact]
    public async Task ListStandardUnitRatesAsync_WhenPeriodFiltersSet_AppendsQueryParameters()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-agile-half-hours-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        TariffChargeListRequest request = new()
        {
            PeriodFrom = new DateTimeOffset(2024, 3, 31, 0, 0, 0, TimeSpan.Zero),
            PeriodTo = new DateTimeOffset(2024, 3, 31, 2, 0, 0, TimeSpan.Zero),
            PageSize = 500,
        };

        await CollectAsync(client.TariffRates.ListStandardUnitRatesAsync(
            TariffCode.Parse(AgileTariff),
            request,
            CancellationToken.None));

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.Contains("period_from=2024-03-31T00%3A00%3A00Z", query, StringComparison.Ordinal);
        Assert.Contains("period_to=2024-03-31T02%3A00%3A00Z", query, StringComparison.Ordinal);
        Assert.Contains("page_size=500", query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListStandardUnitRatesAsync_WhenPageSizeIsZero_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        TariffChargeListRequest request = new() { PageSize = 0 };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.TariffRates.ListStandardUnitRatesAsync(
                TariffCode.Parse(AgileTariff),
                request,
                CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListStandardUnitRatesAsync_WhenPageSizeExceedsMaximum_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        TariffChargeListRequest request = new() { PageSize = RestPageSizeLimits.RatesMaximum + 1 };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.TariffRates.ListStandardUnitRatesAsync(
                TariffCode.Parse(AgileTariff),
                request,
                CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListStandingChargesAsync_WhenGasTariff_UsesGasPath()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-standing-charges-gas.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TariffCharge> charges = await CollectAsync(
            client.TariffRates.ListStandingChargesAsync(TariffCode.Parse(GasTariff), cancellationToken: CancellationToken.None));

        Assert.Single(charges);
        Assert.Equal(28.77m, charges[0].ValueIncVat);
        Assert.EndsWith("/gas-tariffs/G-1R-VAR-22-11-01-N/standing-charges/", handler.SentRequests[0].RequestUri?.AbsolutePath, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListDayUnitRatesAsync_WhenEconomySeven_UsesDayPath()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-go-long-window.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await CollectAsync(client.TariffRates.ListDayUnitRatesAsync(
            TariffCode.Parse(EconomySevenTariff),
            cancellationToken: CancellationToken.None));

        Assert.EndsWith("/day-unit-rates/", handler.SentRequests[0].RequestUri?.AbsolutePath, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListNightUnitRatesAsync_WhenEconomySeven_UsesNightPath()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-go-long-window.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await CollectAsync(client.TariffRates.ListNightUnitRatesAsync(
            TariffCode.Parse(EconomySevenTariff),
            cancellationToken: CancellationToken.None));

        Assert.EndsWith("/night-unit-rates/", handler.SentRequests[0].RequestUri?.AbsolutePath, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListDayUnitRatesAsync_WhenGasTariff_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.TariffRates.ListDayUnitRatesAsync(
                TariffCode.Parse(GasTariff),
                cancellationToken: CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListStandingChargesAsync_WhenValidToNull_ReturnsOpenEndedCharge()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-standing-charges-open-ended.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TariffCharge> charges = await CollectAsync(
            client.TariffRates.ListStandingChargesAsync(TariffCode.Parse(AgileTariff), cancellationToken: CancellationToken.None));

        Assert.Single(charges);
        Assert.Null(charges[0].ValidTo);
        Assert.Equal(39.535125m, charges[0].ValueIncVat);
    }

    [Fact]
    public async Task ListChargesAsync_WhenGoTariff_ReturnsLongValidWindow()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("tariff-rates-go-long-window.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<TariffCharge> rates = await CollectAsync(client.TariffRates.ListChargesAsync(
            TariffCode.Parse(EconomySevenTariff),
            TariffChargeKind.StandardUnitRates,
            cancellationToken: CancellationToken.None));

        Assert.Single(rates);
        Assert.Equal(new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero), rates[0].ValidTo);
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
