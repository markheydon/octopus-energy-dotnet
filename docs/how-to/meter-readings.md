# Meter readings

Submit electricity or gas readings and query smart-meter reading consent through GraphQL using the same dashboard API key as REST.

## When to use this guide

You want to record manual or supplementary readings, or check consent granularity before submitting, without authoring GraphQL documents yourself. The SDK calls `createElectricityMeterReading`, `createGasMeterReading`, and `readingConsentGranularity` internally.

The gas mutation is implemented from the public Kraken schema and unit-test fixtures. The [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md) gas row is still **Not probed** on the maintainer account (no gas meter). Re-probe on a gas-capable dashboard key before treating runtime allow-list status as verified.

## Prerequisites

- Dashboard API key — see [authentication](authentication.md)
- MPAN / MPRN and meter serial numbers — for example from [account detail](accounts.md)
- Reading date as `DateTimeOffset` (the SDK sends the UK civil date to Kraken)

## Reading consent granularity

```csharp
using OctopusEnergy.Client;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

ReadingConsentGranularity consent = await client.MeterReadings.GetReadingConsentGranularityAsync(
    mpan: "1234567890123",
    cancellationToken);

Console.WriteLine(consent.GranularityValue);
```

## Submit an electricity reading

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.MeterReadings;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

ElectricityMeterReadingCreate request = new()
{
    Mpan = "1234567890123",
    SerialNumber = "A1B2C3D4",
    ReadAt = DateTimeOffset.UtcNow,
    Readings =
    [
        new ElectricityRegisterReading { Reading = 12345 },
    ],
};

ElectricityMeterReadingResult result = await client.MeterReadings.CreateElectricityAsync(
    request,
    cancellationToken);

if (result.ReadingErrors.Count > 0)
{
    foreach (MeterReadingFieldError fieldError in result.ReadingErrors)
    {
        foreach (string message in fieldError.Errors)
        {
            Console.WriteLine($"{fieldError.Field}: {message}");
        }
    }
}
```

## Submit a gas reading

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.MeterReadings;

GasMeterReadingCreate gasRequest = new()
{
    Mprn = "1234567890",
    SerialNumber = "G1A2S3",
    ReadAt = DateTimeOffset.UtcNow,
    Reading = 54321,
};

GasMeterReadingResult gasResult = await client.MeterReadings.CreateGasAsync(
    gasRequest,
    cancellationToken);
```

## Errors

- Missing API key: `OctopusEnergyRequestException` before any HTTP call.
- Kraken GraphQL failures (`errors.extensions.errorCode`, for example `KT-GB-11007`): `OctopusEnergyKrakenException`. See [error handling](error-handling.md).
- Field validation inside the mutation payload appears in `ElectricityMeterReadingResult.ReadingErrors` or `GasMeterReadingResult.ReadingErrors` when GraphQL returns HTTP 200 with no top-level `errors`.

## Related

- [Account detail](accounts.md) — MPAN, MPRN, and serial discovery via REST
- [REST and GraphQL](../explanation/rest-and-graphql.md)
- [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md)
