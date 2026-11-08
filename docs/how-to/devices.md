# SmartFlex devices and planned dispatches

Discover Intelligent / SmartFlex hardware and read upcoming dispatch windows through GraphQL using the same dashboard API key as REST.

## When to use this guide

You need device ids and planned charge or discharge slots without authoring GraphQL documents. The SDK queries `devices` and `flexPlannedDispatches` only (not deprecated `plannedDispatches`).

## Prerequisites

- Dashboard API key — see [authentication](authentication.md)
- Account number — see [accounts](accounts.md) or `client.Viewer` when you do not have one yet
- Enrolled SmartFlex hardware on the account (otherwise `devices` returns an empty list)

## List devices

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Devices;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

IReadOnlyList<SmartFlexDevice> devices = await client.Devices.ListDevicesAsync(
    "A-12345678",
    cancellationToken);

foreach (SmartFlexDevice device in devices)
{
    Console.WriteLine($"{device.Id} {device.TypeName} {device.CurrentState}");
}
```

An empty list means the account has no SmartFlex devices registered — not an error.

## Planned dispatches

Use a device `Id` from the list above:

```csharp
IReadOnlyList<FlexPlannedDispatch> dispatches = await client.Devices.ListFlexPlannedDispatchesAsync(
    devices[0].Id!,
    cancellationToken);

foreach (FlexPlannedDispatch dispatch in dispatches)
{
    Console.WriteLine($"{dispatch.Start} → {dispatch.End} {dispatch.Type} {dispatch.EnergyAddedKwh} kWh");
}
```

Negative `EnergyAddedKwh` values indicate export or discharge slots. If the device id is not entitled for this API key, Kraken returns `KT-CT-1111` and the SDK throws `OctopusEnergyKrakenException`.
