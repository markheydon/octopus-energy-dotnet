# Electricity meter readings

Submit an electricity meter reading through GraphQL using the same dashboard API key as REST.

## When to use this guide

You want to record a manual or supplementary electricity reading without authoring GraphQL documents yourself. The SDK calls `createElectricityMeterReading` internally.

Gas meter readings are not yet on the public surface; the allow-list spike did not verify them on the probe account. See [API coverage](../reference/api-coverage.md).

## Prerequisites

- Dashboard API key — see [authentication](authentication.md)
- Import MPAN and electricity meter serial number (for example from [account detail](accounts.md))
- Reading date as `DateTimeOffset` (the SDK sends the UK civil date to Kraken)

## Submit a reading

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

## Errors

- Missing API key: `OctopusEnergyRequestException` before any HTTP call.
- Kraken GraphQL failures (`errors.extensions.errorCode`, for example `KT-GB-11007`): `OctopusEnergyKrakenException`. See [error handling](error-handling.md).
- Field validation inside the mutation payload appears in `ElectricityMeterReadingResult.ReadingErrors` when GraphQL returns HTTP 200 with no top-level `errors`.

## Related

- [Account detail](accounts.md) — MPAN and serial discovery via REST
- [REST and GraphQL](../explanation/rest-and-graphql.md)
- [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md)
