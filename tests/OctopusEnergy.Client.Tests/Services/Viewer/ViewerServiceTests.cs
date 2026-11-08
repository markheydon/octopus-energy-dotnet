using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Viewer;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Viewer;

public sealed class ViewerServiceTests
{
    [Fact]
    public async Task GetViewerAsync_WhenAuthenticated_ReturnsDistinctAccountNumbers()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-viewer.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ViewerDetail viewer = await client.Viewer.GetViewerAsync(CancellationToken.None);

        Assert.Equal("viewer-id-redacted", viewer.Id);
        Assert.Equal(["A-TEST0001", "A-TEST0002", "A-TEST0003"], viewer.AccountNumbers);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task GetViewerAsync_WhenNoAccounts_ReturnsEmptyList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-viewer-empty.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ViewerDetail viewer = await client.Viewer.GetViewerAsync(CancellationToken.None);

        Assert.Empty(viewer.AccountNumbers);
    }

    [Fact]
    public async Task GetViewerAsync_WhenPortfoliosOverlap_DeduplicatesWithDirectAccountsFirst()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-viewer-portfolios.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ViewerDetail viewer = await client.Viewer.GetViewerAsync(CancellationToken.None);

        Assert.Equal(["A-TEST0001", "A-TEST0003"], viewer.AccountNumbers);
    }

    [Fact]
    public async Task GetViewerAsync_WhenViewerNull_ThrowsParseException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, """{"data":{"viewer":null},"errors":null}""");

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyParseException>(
            () => client.Viewer.GetViewerAsync(CancellationToken.None));
    }

    [Fact]
    public async Task GetViewerAsync_WhenKrakenError_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1188.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        OctopusEnergyKrakenException exception = await Assert.ThrowsAsync<OctopusEnergyKrakenComplexityException>(
            () => client.Viewer.GetViewerAsync(CancellationToken.None));

        Assert.Equal("KT-CT-1188", exception.KrakenErrorCode);
    }

    [Fact]
    public async Task GetViewerAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Viewer.GetViewerAsync(CancellationToken.None));
    }
}
