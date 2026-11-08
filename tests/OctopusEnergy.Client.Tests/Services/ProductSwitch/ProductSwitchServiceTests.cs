using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.ProductSwitch;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.ProductSwitch;

public sealed class ProductSwitchServiceTests
{
    private const string AccountNumber = "A-TEST0001";
    private const string QuotedProductId = "75085546";

    private static readonly ProductSwitchInitiate SampleRequest = new()
    {
        AccountNumber = AccountNumber,
        SwitchDate = new DateOnly(2026, 4, 1),
        QuotedProductId = QuotedProductId,
    };

    [Fact]
    public async Task InitiateAsync_WhenSuccessful_ReturnsProductCodeAndSwitchDate()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-initiate-product-switch-success.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ProductSwitchResult result = await client.ProductSwitch.InitiateAsync(SampleRequest, CancellationToken.None);

        Assert.Equal("E-1R-VAR-22-11-25-C", result.ProductCode);
        Assert.Equal(new DateOnly(2026, 4, 1), result.SwitchDate);
        Assert.Contains("\"quotedProductId\":\"75085546\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
        Assert.Contains("\"switchDate\":\"2026-04-01\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task InitiateAsync_WhenMutationPayloadNull_ThrowsParseException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-initiate-product-switch-null.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyParseException>(
            () => client.ProductSwitch.InitiateAsync(SampleRequest, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WhenKrakenError_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-initiate-product-switch-invalid.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => client.ProductSwitch.InitiateAsync(SampleRequest, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.ProductSwitch.InitiateAsync(SampleRequest, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WhenAccountNumberMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        ProductSwitchInitiate request = new()
        {
            AccountNumber = " ",
            SwitchDate = new DateOnly(2026, 4, 1),
            QuotedProductId = QuotedProductId,
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.ProductSwitch.InitiateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WhenBothQuoteIdentifiers_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        ProductSwitchInitiate request = new()
        {
            AccountNumber = AccountNumber,
            SwitchDate = new DateOnly(2026, 4, 1),
            QuotedProductId = QuotedProductId,
            OfferIdentifier = "65485614",
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.ProductSwitch.InitiateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WhenNeitherQuoteIdentifier_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        ProductSwitchInitiate request = new()
        {
            AccountNumber = AccountNumber,
            SwitchDate = new DateOnly(2026, 4, 1),
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.ProductSwitch.InitiateAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task InitiateAsync_WithOfferIdentifier_SendsOfferOnWire()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-initiate-product-switch-success.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ProductSwitchInitiate request = new()
        {
            AccountNumber = AccountNumber,
            SwitchDate = new DateOnly(2026, 4, 1),
            OfferIdentifier = "65485614",
        };

        await client.ProductSwitch.InitiateAsync(request, CancellationToken.None);

        Assert.Contains("\"offerIdentifier\":\"65485614\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }
}
