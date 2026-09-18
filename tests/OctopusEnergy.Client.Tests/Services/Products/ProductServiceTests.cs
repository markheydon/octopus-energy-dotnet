using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Products;

public sealed class ProductServiceTests
{
    [Fact]
    public async Task ListAsync_WhenTwoPages_ReturnsAllProducts()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<Product> products = await CollectAsync(client.Products.ListAsync(cancellationToken: CancellationToken.None));

        Assert.Equal(["AGILE-FLEX-22-11-25", "OTHER-BRAND-PRODUCT", "VAR-22-11-01"], products.Select(product => product.Code));
        Assert.EndsWith("/products/", handler.SentRequests[0].RequestUri?.AbsolutePath, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListAsync_WhenNoRequest_SendsNoBrandFilter()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await CollectAsync(client.Products.ListAsync(cancellationToken: CancellationToken.None));

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.True(string.IsNullOrEmpty(query));
    }

    [Fact]
    public async Task ListAsync_WhenFiltersSet_AppendsQueryParameters()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductListRequest request = new()
        {
            Brand = "OCTOPUS_ENERGY",
            IsVariable = false,
            IsGreen = true,
            IsTracker = false,
            IsPrepay = true,
            IsBusiness = false,
            AvailableAt = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
        };

        await CollectAsync(client.Products.ListAsync(request, CancellationToken.None));

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.NotNull(query);
        Assert.Contains("brand=OCTOPUS_ENERGY", query, StringComparison.Ordinal);
        Assert.Contains("is_variable=false", query, StringComparison.Ordinal);
        Assert.Contains("is_green=true", query, StringComparison.Ordinal);
        Assert.Contains("is_tracker=false", query, StringComparison.Ordinal);
        Assert.Contains("is_prepay=true", query, StringComparison.Ordinal);
        Assert.Contains("is_business=false", query, StringComparison.Ordinal);
        Assert.Contains("available_at=2019-01-01T00%3A00%3A00Z", query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListAsync_WhenMultipleBrands_ReturnsDistinctBrands()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-1.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-2.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        List<Product> products = await CollectAsync(client.Products.ListAsync(cancellationToken: CancellationToken.None));

        Assert.Contains(products, product => product.Brand == "OCTOPUS_ENERGY");
        Assert.Contains(products, product => product.Brand == "OTHER_BRAND");
    }

    [Fact]
    public async Task GetAsync_WhenValidCode_ReturnsProductDetail()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        Assert.Equal("AGILE-FLEX-22-11-25", detail.Code);
        Assert.Equal(new DateTimeOffset(2023, 11, 10, 0, 21, 44, 970, TimeSpan.Zero).AddTicks(7110), detail.TariffsActiveAt);
        Assert.Equal("/v1/products/AGILE-FLEX-22-11-25/", handler.SentRequests[0].RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task GetAsync_WhenTariffsActiveAtSet_AppendsQueryParameter()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        DateTimeOffset activeAt = new(2019, 1, 1, 0, 0, 0, TimeSpan.Zero);
        await client.Products.GetAsync("AGILE-FLEX-22-11-25", activeAt, CancellationToken.None);

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.Contains("tariffs_active_at=2019-01-01T00%3A00%3A00Z", query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_WhenMultipleGspBlocks_DeserialisesTariffsByRegion()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        Assert.True(detail.SingleRegisterElectricityTariffs.ContainsKey(GridSupplyPoint.A));
        Assert.True(detail.SingleRegisterElectricityTariffs.ContainsKey(GridSupplyPoint.C));

        ProductTariff regionA = detail.SingleRegisterElectricityTariffs[GridSupplyPoint.A].DirectDebitMonthly!;
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-A", regionA.Code);
        Assert.Equal(18.6585m, regionA.StandardUnitRateIncVat);

        ProductTariff regionC = detail.SingleRegisterElectricityTariffs[GridSupplyPoint.C].DirectDebitMonthly!;
        Assert.Equal("E-1R-AGILE-FLEX-22-11-25-C", regionC.Code);
    }

    [Fact]
    public async Task GetAsync_WhenPaymentMethodObjectEmpty_DeserialisesAsNull()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        ProductPaymentMethodTariffs regionA = detail.SingleRegisterElectricityTariffs[GridSupplyPoint.A];
        Assert.NotNull(regionA.DirectDebitMonthly);
        Assert.Null(regionA.DirectDebitQuarterly);
    }

    [Fact]
    public async Task GetAsync_WhenSampleDataPresent_DeserialisesQuotesAndConsumption()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        Assert.True(detail.SampleQuotes.ContainsKey(GridSupplyPoint.A));
        Assert.Equal(
            90000m,
            detail.SampleQuotes[GridSupplyPoint.A].DirectDebitMonthly!.ElectricitySingleRate!.AnnualCostIncVat);
        Assert.Equal(3100m, detail.SampleConsumption!.ElectricitySingleRate!.ElectricityStandard);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("products-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Products.GetAsync("MISSING-PRODUCT", cancellationToken: CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Not found.", exception.Detail);
    }

    [Fact]
    public async Task ListAsync_WhenBrandWhitespace_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductListRequest request = new() { Brand = "   " };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => CollectAsync(client.Products.ListAsync(request, CancellationToken.None)));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task ListAsync_WhenSecondPageFails_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-list-page-1.json"));
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("products-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            async () => await CollectAsync(client.Products.ListAsync(cancellationToken: CancellationToken.None)));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Not found.", exception.Detail);
    }

    [Fact]
    public async Task GetAsync_WhenTariffsActiveAtHasFraction_AppendsFractionalQueryParameter()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        DateTimeOffset activeAt = new DateTimeOffset(2023, 11, 10, 0, 21, 44, 970, TimeSpan.Zero).AddTicks(7110);
        await client.Products.GetAsync("AGILE-FLEX-22-11-25", activeAt, CancellationToken.None);

        string? query = handler.SentRequests[0].RequestUri?.Query;
        Assert.Contains("tariffs_active_at=2023-11-10T00%3A21%3A44.970711Z", query, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_WhenProductCodeNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Products.GetAsync(null!, cancellationToken: CancellationToken.None));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_WhenTariffMapNull_DeserialisesEmptyDictionary()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.OK,
            """{"code":"AGILE-FLEX-22-11-25","single_register_electricity_tariffs":null}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        Assert.NotNull(detail.SingleRegisterElectricityTariffs);
        Assert.Empty(detail.SingleRegisterElectricityTariffs);
    }

    [Fact]
    public async Task GetAsync_WhenUnknownGspKeyPresent_DeserialisesKnownRegions()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.OK,
            """
            {
              "code": "AGILE-FLEX-22-11-25",
              "single_register_electricity_tariffs": {
                "_Q": { "direct_debit_monthly": { "code": "UNKNOWN" } },
                "_C": { "direct_debit_monthly": { "code": "E-1R-AGILE-FLEX-22-11-25-C" } }
              }
            }
            """);

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        ProductDetail detail = await client.Products.GetAsync("AGILE-FLEX-22-11-25", cancellationToken: CancellationToken.None);

        Assert.Single(detail.SingleRegisterElectricityTariffs);
        Assert.True(detail.SingleRegisterElectricityTariffs.ContainsKey(GridSupplyPoint.C));
    }

    [Fact]
    public async Task GetAsync_WhenProductCodeEmpty_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Products.GetAsync("   ", cancellationToken: CancellationToken.None));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_WhenProductCodeHasSpecialCharacters_EscapesPath()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("products-agile-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await client.Products.GetAsync("AGILE/FLEX", cancellationToken: CancellationToken.None);

        Assert.Contains("AGILE%2FFLEX", handler.SentRequests[0].RequestUri?.AbsoluteUri, StringComparison.Ordinal);
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
