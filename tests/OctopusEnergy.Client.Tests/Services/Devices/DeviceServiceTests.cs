using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Devices;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Devices;

public sealed class DeviceServiceTests
{
    private const string AccountNumber = "A-TEST0001";
    private const string DeviceId = "00000000-0000-4000-8000-000000000001";

    [Fact]
    public async Task ListDevicesAsync_WhenAuthenticated_ReturnsDevices()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-devices-page.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<SmartFlexDevice> devices = await client.Devices.ListDevicesAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.Equal(2, devices.Count);
        Assert.Equal(DeviceId, devices[0].Id);
        Assert.Equal("SmartFlexChargePoint", devices[0].TypeName);
        Assert.Equal("IDLE", devices[0].CurrentState);
        Assert.Equal("SmartFlexVehicle", devices[1].TypeName);
        Assert.Contains("\"accountNumber\":\"A-TEST0001\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListDevicesAsync_WhenEmpty_ReturnsEmptyList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-devices-empty.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<SmartFlexDevice> devices = await client.Devices.ListDevicesAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.Empty(devices);
    }

    [Fact]
    public async Task ListDevicesAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Devices.ListDevicesAsync(AccountNumber, CancellationToken.None));
    }

    [Fact]
    public async Task ListDevicesAsync_WhenAccountNumberMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Devices.ListDevicesAsync(" ", CancellationToken.None));
    }

    [Fact]
    public async Task ListDevicesAsync_WhenKrakenError_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-error-kt-ct-1188.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyKrakenComplexityException>(
            () => client.Devices.ListDevicesAsync(AccountNumber, CancellationToken.None));
    }

    [Fact]
    public async Task ListFlexPlannedDispatchesAsync_WhenAuthenticated_ReturnsDispatches()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-flex-planned-dispatches.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<FlexPlannedDispatch> dispatches = await client.Devices.ListFlexPlannedDispatchesAsync(
            DeviceId,
            CancellationToken.None);

        Assert.Equal(2, dispatches.Count);
        Assert.Equal("SMART", dispatches[0].Type);
        Assert.Equal(12.5m, dispatches[0].EnergyAddedKwh);
        Assert.Equal(-2.0m, dispatches[1].EnergyAddedKwh);
        Assert.Contains($"\"deviceId\":\"{DeviceId}\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
        Assert.DoesNotContain("plannedDispatches", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListFlexPlannedDispatchesAsync_WhenEmpty_ReturnsEmptyList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-flex-planned-dispatches-empty.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<FlexPlannedDispatch> dispatches = await client.Devices.ListFlexPlannedDispatchesAsync(
            DeviceId,
            CancellationToken.None);

        Assert.Empty(dispatches);
    }

    [Fact]
    public async Task ListFlexPlannedDispatchesAsync_WhenDeviceIdMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Devices.ListFlexPlannedDispatchesAsync(" ", CancellationToken.None));
    }

    [Fact]
    public async Task ListFlexPlannedDispatchesAsync_WhenUnauthorized_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-flex-dispatches-denied.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        OctopusEnergyKrakenException exception = await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => client.Devices.ListFlexPlannedDispatchesAsync(DeviceId, CancellationToken.None));

        Assert.Equal("KT-CT-1111", exception.KrakenErrorCode);
    }
}
