# Home Mini smart-meter telemetry

Read near-real-time SMETS telemetry and discover EUI64 device ids through GraphQL using the same dashboard API key as REST.

## When to use this guide

You want Home Mini / CAD telemetry without authoring GraphQL documents or walking the `account → agreements → smartDevices` chain by hand.

## Prerequisites

- NuGet **1.2.x** or a build from `main` (not package **1.1.0**) — [API coverage](../reference/api-coverage.md)
- Dashboard API key — see [authentication](authentication.md)
- Account number — see [accounts](accounts.md) or `client.Viewer` when you do not have one yet
- SMETS hardware with telemetry entitlement (otherwise discovery returns empty lists or telemetry calls return `KT-CT-1111`)

## Discover EUI64 device ids

The SDK queries `account → electricityAgreements | gasAgreements → meterPoint → meters → smartDevices` and deduplicates by `deviceId`:

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.SmartMeterTelemetry;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

IReadOnlyList<SmartMeterDevice> devices = await client.SmartMeterTelemetry.ListSmartDevicesAsync(
    "A-12345678",
    cancellationToken);

foreach (SmartMeterDevice device in devices)
{
    Console.WriteLine($"{device.DeviceId} {device.DeviceType} {device.SupplyPointIdentifier}");
}
```

An empty list means no smart devices were returned for the account — not an error.

## Latest telemetry

Pass the EUI64 from discovery (for example `12-34-5A-FF-FF-6B-C7-89`):

```csharp
IReadOnlyList<SmartMeterTelemetryReading> latest = await client.SmartMeterTelemetry.GetTelemetryAsync(
    devices[0].DeviceId,
    cancellationToken);

foreach (SmartMeterTelemetryReading sample in latest)
{
    Console.WriteLine($"{sample.ReadAt} Δ{sample.ConsumptionDelta} kWh demand {sample.Demand}");
}
```

## Ranged telemetry

`start`, `end`, and `grouping` must be supplied together:

```csharp
SmartMeterTelemetryRange range = new()
{
    Start = DateTimeOffset.Parse("2024-06-01T11:00:00+00:00"),
    End = DateTimeOffset.Parse("2024-06-01T12:00:00+00:00"),
    Grouping = SmartMeterTelemetryGrouping.FiveMinutes,
};

IReadOnlyList<SmartMeterTelemetryReading> series = await client.SmartMeterTelemetry.GetTelemetryAsync(
    devices[0].DeviceId,
    range,
    cancellationToken);
```

## Rate limits and errors

Kraken documents a **125 calls per hour** limit on `smartMeterTelemetry` per user. Over-polling also surfaces field rate limits as `KT-CT-1199` (`OctopusEnergyKrakenFieldRateLimitException`). The SDK does **not** retry automatically — back off in your application (for example poll dashboards every few minutes, not every second). See [error handling](error-handling.md).

For hourly GraphQL **complexity points**, use `client.RateLimits.GetRateLimitInfoAsync` alongside conservative telemetry polling.

Unauthorised or unknown device ids throw `OctopusEnergyKrakenException` (for example `KT-CT-1111`).
