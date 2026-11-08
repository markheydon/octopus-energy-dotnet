using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.MeterReadings;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.MeterReadings;

public sealed class MeterReadingServiceTests
{
    private static readonly ElectricityMeterReadingCreate SampleRequest = new()
    {
        Mpan = "1234567890123",
        SerialNumber = "SERIAL123",
        ReadAt = new DateTimeOffset(2024, 6, 15, 10, 0, 0, TimeSpan.FromHours(1)),
        Readings = [new ElectricityRegisterReading { Reading = 12345 }],
    };

    [Fact]
    public async Task CreateElectricityAsync_WhenSuccessful_ReturnsEmptyReadingErrors()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-create-electricity-meter-reading-success.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ElectricityMeterReadingResult result = await client.MeterReadings.CreateElectricityAsync(
            SampleRequest,
            CancellationToken.None);

        Assert.Empty(result.ReadingErrors);
        Assert.Equal(2, handler.SentRequests.Count);
    }

    [Fact]
    public async Task CreateElectricityAsync_WhenFieldErrors_ReturnsMappedErrors()
    {
        const string body = """
            {
              "data": {
                "createElectricityMeterReading": {
                  "readingErrors": [
                    {
                      "field": "readings",
                      "errors": ["Reading value is invalid."]
                    }
                  ]
                }
              },
              "errors": null
            }
            """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, body);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ElectricityMeterReadingResult result = await client.MeterReadings.CreateElectricityAsync(
            SampleRequest,
            CancellationToken.None);

        Assert.Single(result.ReadingErrors);
        Assert.Equal("readings", result.ReadingErrors[0].Field);
        Assert.Equal(["Reading value is invalid."], result.ReadingErrors[0].Errors);
    }

    [Fact]
    public async Task CreateElectricityAsync_WhenKrakenError_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-create-electricity-reading.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        OctopusEnergyKrakenException exception = await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => client.MeterReadings.CreateElectricityAsync(SampleRequest, CancellationToken.None));

        Assert.Equal("KT-GB-11007", exception.KrakenErrorCode);
    }

    [Fact]
    public async Task CreateElectricityAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.MeterReadings.CreateElectricityAsync(SampleRequest, CancellationToken.None));
    }

    [Fact]
    public async Task CreateElectricityAsync_WhenMpanMissing_ThrowsBeforeHttpCall()
    {
        using HttpClient httpClient = new() { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ElectricityMeterReadingCreate request = new()
        {
            Mpan = " ",
            SerialNumber = "SERIAL123",
            ReadAt = SampleRequest.ReadAt,
            Readings = SampleRequest.Readings,
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.MeterReadings.CreateElectricityAsync(request, CancellationToken.None));
    }

    [Fact]
    public async Task CreateElectricityAsync_WhenReadingsEmpty_ThrowsBeforeHttpCall()
    {
        using HttpClient httpClient = new() { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        ElectricityMeterReadingCreate request = new()
        {
            Mpan = SampleRequest.Mpan,
            SerialNumber = SampleRequest.SerialNumber,
            ReadAt = SampleRequest.ReadAt,
            Readings = [],
        };

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.MeterReadings.CreateElectricityAsync(request, CancellationToken.None));
    }
}
