# Account detail

Fetch your account properties, meter points, meters, registers, and tariff agreements. Account calls require a dashboard API key. You supply the account number; discovering it without a bill is a v2 GraphQL `viewer` story.

## Prerequisites

- A dashboard API key from [API access](https://octopus.energy/dashboard/new/accounts/personal-details/api-access).
- Your account number (for example `A-12345678`), from your bill or dashboard.

## Fetch account detail

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;

const string apiKey = "sk_test_not_a_real_key";
const string accountNumber = "A-12345678";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

Account account = await client.Accounts.GetAsync(accountNumber, cancellationToken);
Console.WriteLine($"{account.Number}: {account.Properties.Count} properties");
```

## Walk properties and meters

```csharp
foreach (AccountProperty property in account.Properties)
{
    Console.WriteLine($"{property.Postcode}: {property.ElectricityMeterPoints.Count} electricity points");

    foreach (ElectricityMeterPoint point in property.ElectricityMeterPoints)
    {
        Console.WriteLine($"  MPAN {point.Mpan}, export={point.IsExport?.ToString() ?? "unknown"}");

        foreach (ElectricityMeter meter in point.Meters)
        {
            Console.WriteLine($"    Serial {meter.SerialNumber}");
        }

        TariffAgreement? current = point.Agreements.FirstOrDefault(agreement => agreement.ValidTo is null);
        if (current is not null)
        {
            Console.WriteLine($"    Current tariff: {current.TariffCode}");
        }
    }
}
```

## Import vs export MPANs

The REST payload includes `is_export` on electricity meter points when the API returns it. When the field is missing, use `TariffCode.Parse` on agreement codes or other context. The API is not always obvious when you have multiple MPANs on one property.

```csharp
if (point.IsExport == true)
{
    // Export MPAN — consumption endpoints still use the field name "consumption".
}
```

## Errors

| HTTP status | Exception | Typical cause |
|---|---|---|
| 401 | `OctopusEnergyApiException` | Missing or invalid API key |
| 403 | `OctopusEnergyApiException` | API access not enabled for the account user |
| 404 | `OctopusEnergyApiException` | Unknown account number |

Empty or whitespace account numbers throw `OctopusEnergyRequestException` before any HTTP call.

## Related

- [Authentication](authentication.md) — API keys and HTTP Basic
- [Tariff codes and GSP](tariff-codes.md) — parse `tariff_code` from agreements
- [API coverage](../reference/api-coverage.md)
