using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.SmartMeterTelemetry;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.SmartMeterTelemetry;

public sealed class SmartMeterTelemetryServiceTests
{
    private const string AccountNumber = "A-TEST0001";
    private const string DeviceId = "12-34-5A-FF-FF-6B-C7-89";

    [Fact]
    public async Task ListSmartDevicesAsync_WhenAuthenticated_ReturnsDevices()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-smart-meter-devices.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<SmartMeterDevice> devices = await client.SmartMeterTelemetry.ListSmartDevicesAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.Single(devices);
        Assert.Equal(DeviceId, devices[0].DeviceId);
        Assert.Equal("ESME", devices[0].DeviceType);
        Assert.Equal("MTR-IMPORT-01", devices[0].MeterSerialNumber);
        Assert.Equal("1234567890123", devices[0].SupplyPointIdentifier);
        Assert.Equal(EnergyFuel.Electricity, devices[0].Fuel);
        Assert.Contains("\"accountNumber\":\"A-TEST0001\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListSmartDevicesAsync_WhenAccountNull_ThrowsParseException()
    {
        const string body = """
      {
        "data": {
          "account": null
        },
        "errors": null
      }
      """;

        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, body);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyParseException>(
            () => client.SmartMeterTelemetry.ListSmartDevicesAsync(AccountNumber, CancellationToken.None));
    }

    [Fact]
    public async Task ListSmartDevicesAsync_WhenEmpty_ReturnsEmptyList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-smart-meter-devices-empty.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<SmartMeterDevice> devices = await client.SmartMeterTelemetry.ListSmartDevicesAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.Empty(devices);
    }

    [Fact]
    public async Task ListSmartDevicesAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.SmartMeterTelemetry.ListSmartDevicesAsync(AccountNumber, CancellationToken.None));
    }

    [Fact]
    public async Task ListSmartDevicesAsync_WhenAccountNumberMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.SmartMeterTelemetry.ListSmartDevicesAsync(" ", CancellationToken.None));
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenLatest_ReturnsReadings()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-smart-meter-telemetry.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<SmartMeterTelemetryReading> readings = await client.SmartMeterTelemetry.GetTelemetryAsync(
            DeviceId,
            CancellationToken.None);

        Assert.Equal(2, readings.Count);
        Assert.Equal(123.456m, readings[0].Consumption);
        Assert.Equal(0.012m, readings[0].ConsumptionDelta);
        Assert.DoesNotContain("FIVE_MINUTES", handler.SentRequestBodies[1], StringComparison.Ordinal);
        Assert.Contains("\"grouping\":null", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenRanged_SendsStartEndAndGrouping()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-smart-meter-telemetry.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        SmartMeterTelemetryRange range = new()
        {
            Start = new DateTimeOffset(2024, 6, 1, 11, 0, 0, TimeSpan.Zero),
            End = new DateTimeOffset(2024, 6, 1, 12, 0, 0, TimeSpan.Zero),
            Grouping = SmartMeterTelemetryGrouping.FiveMinutes,
        };

        await client.SmartMeterTelemetry.GetTelemetryAsync(DeviceId, range, CancellationToken.None);

        string body = handler.SentRequestBodies[1];
        Assert.Contains("\"FIVE_MINUTES\"", body, StringComparison.Ordinal);
        Assert.Contains("\"deviceId\"", body, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenDeviceIdMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.SmartMeterTelemetry.GetTelemetryAsync(" ", CancellationToken.None));
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenRangeNull_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => client.SmartMeterTelemetry.GetTelemetryAsync(DeviceId, range: null!, CancellationToken.None));
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenFieldRateLimited_ThrowsFieldRateLimitException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1199.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyKrakenFieldRateLimitException>(
            () => client.SmartMeterTelemetry.GetTelemetryAsync(DeviceId, CancellationToken.None));
    }

    [Fact]
    public async Task GetTelemetryAsync_WhenUnauthorizedDevice_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-smart-meter-telemetry-denied.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        OctopusEnergyKrakenException exception = await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => client.SmartMeterTelemetry.GetTelemetryAsync(DeviceId, CancellationToken.None));

        Assert.Equal("KT-CT-1111", exception.KrakenErrorCode);
    }
}
