# Account detail

Fetch properties, electricity and gas meter points, meters, registers, and tariff agreements for a customer account.

## When to use this guide

You have a dashboard API key. Use GraphQL viewer discovery when you do not yet have an account number (for example from a bill); use REST account detail when you already know `A-XXXXXXXX`.

## Prerequisites

- Dashboard API key - see [authentication](authentication.md)
- Account number in the form `A-12345678` (for REST fetch below)

## Discover account numbers

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Viewer;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

ViewerDetail viewer = await client.Viewer.GetViewerAsync(cancellationToken);

if (viewer.AccountNumbers.Count == 0)
{
    Console.WriteLine("No accounts linked to this API key.");
}
else if (viewer.AccountNumbers.Count > 1)
{
    foreach (string number in viewer.AccountNumbers)
    {
        Console.WriteLine(number);
    }
    // Choose the account you need; the SDK does not pick a default.
}
else
{
    Console.WriteLine(viewer.AccountNumbers[0]);
}
```

## Fetch account detail

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

Account account = await client.Accounts.GetAsync("A-12345678", cancellationToken);
Console.WriteLine($"{account.Number}: {account.Properties.Count} properties");

foreach (AccountProperty property in account.Properties)
{
    Console.WriteLine(property.AddressLine1);
    foreach (ElectricityMeterPoint mp in property.ElectricityMeterPoints)
    {
        Console.WriteLine($"  MPAN {mp.Mpan}: {mp.Agreements.Count} agreements");
    }
}
```

## What the response contains

Each `AccountProperty` includes address fields, `ElectricityMeterPoints`, and `GasMeterPoints`. Meter points carry MPAN/MPRN, GSP, agreements (tariff codes and date ranges), and nested meters with registers.

Use tariff codes from agreements with [tariff codes and GSP](tariff-codes.md) and [tariff rates](tariff-rates.md) for standing charges and unit-rate history. Consumption intervals are on `client.Consumption` — see [consumption](consumption.md).

## Import vs export MPANs

The REST payload includes `is_export` on electricity meter points when the API returns it. When the field is missing, use `TariffCode.Parse` on agreement codes or other context. The API is not always obvious when you have multiple MPANs on one property.

```csharp
if (point.IsExport == true)
{
    // Export MPAN - consumption endpoints still use the field name "consumption".
}
```

## Errors

| HTTP status | Exception | Typical cause |
|---|---|---|
| 401 | `OctopusEnergyApiException` | Missing or invalid API key |
| 403 | `OctopusEnergyApiException` | API access not enabled for the account user |
| 404 | `OctopusEnergyApiException` | Unknown account number |

Empty or whitespace account numbers throw `OctopusEnergyRequestException` before any HTTP call.

See [error handling](error-handling.md).

## Related

- [Authentication](authentication.md)
- [Consumption](consumption.md)
- [Tariff rates](tariff-rates.md)
- [Units, VAT, and time](../explanation/units-vat-and-time.md)
- [API coverage](../reference/api-coverage.md)
