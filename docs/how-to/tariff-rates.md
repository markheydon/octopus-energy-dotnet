# Standing charges and unit-rate history

List standing charges and unit-rate history for a parsed `TariffCode`. These are public endpoints and do not require an API key.

## When to use this guide

You have a tariff code — from an account agreement, product detail, or `TariffCode.Parse` — and need rate history rather than the snapshot values on `ProductTariff`. See [tariff codes and GSP](tariff-codes.md) for parsing and GSP mapping.

## Prerequisites

- A parsed `TariffCode` (for example `E-1R-AGILE-FLEX-22-11-25-C`)
- No API key required for rate history endpoints

## Standing charges

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;

CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient();

TariffCode tariff = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");

await foreach (TariffCharge charge in client.TariffRates.ListStandingChargesAsync(
    tariff,
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"{charge.ValidFrom}: {charge.ValueIncVat} p/day");
}
```

Standing charges are in **pence per day**. Each `TariffCharge` exposes `ValueExcVat` and `ValueIncVat`.

## Standard unit rates

```csharp
await foreach (TariffCharge rate in client.TariffRates.ListStandardUnitRatesAsync(
    tariff,
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"{rate.ValidFrom}: {rate.ValueIncVat} p/kWh");
}
```

Unit rates are in **pence per kWh**. Agile and other half-hourly tariffs return many rows per day.

## Economy 7 day and night rates

Dual-register electricity tariffs (`E-2R-…`) use separate day and night endpoints:

```csharp
TariffCode economy7 = TariffCode.Parse("E-2R-VAR-22-11-01-A");

await foreach (TariffCharge dayRate in client.TariffRates.ListDayUnitRatesAsync(
    economy7,
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"Day: {dayRate.ValueIncVat} p/kWh");
}

await foreach (TariffCharge nightRate in client.TariffRates.ListNightUnitRatesAsync(
    economy7,
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"Night: {nightRate.ValueIncVat} p/kWh");
}
```

Gas tariffs and single-register electricity tariffs support standing charges and standard unit rates only. `ListDayUnitRatesAsync` and `ListNightUnitRatesAsync` throw `OctopusEnergyRequestException` for gas or single-register electricity before any HTTP call.

## Filter by period

Pass a `TariffChargeListRequest` to limit the window and control page size:

```csharp
TariffChargeListRequest request = new()
{
    PeriodFrom = new DateTimeOffset(2024, 3, 31, 0, 0, 0, TimeSpan.Zero),
    PeriodTo = new DateTimeOffset(2024, 3, 31, 2, 0, 0, TimeSpan.Zero),
    PageSize = 500,
};

await foreach (TariffCharge rate in client.TariffRates.ListStandardUnitRatesAsync(
    tariff,
    request,
    cancellationToken))
{
    // ...
}
```

`PageSize` must be between 1 and `RestPageSizeLimits.RatesMaximum` (1,500). Invalid values throw `OctopusEnergyRequestException` before any HTTP call.

Pagination is automatic. The SDK follows REST `next` links until all pages are returned. See [pagination](pagination.md).

## Reading results

Each `TariffCharge` row has:

- `ValidFrom` and `ValidTo` — the rate period; `ValidTo == null` means open-ended
- `ValueExcVat` and `ValueIncVat` — use the field that matches your display or reconciliation need
- `PaymentMethod` — when the API returns it

Agile unit rates are in **UTC**. When joining rates to consumption intervals, match on `DateTimeOffset` instants, not wall-clock hour. See [units, VAT, and time](../explanation/units-vat-and-time.md).

## Get a tariff code from account detail

Tariff codes on agreements can be parsed directly:

```csharp
using OctopusEnergy.Client.Models.Accounts;

TariffAgreement agreement = point.Agreements[0];
TariffCode? code = agreement.ParsedTariffCode;

if (code is not null)
{
    await foreach (TariffCharge rate in client.TariffRates.ListStandardUnitRatesAsync(
        code.Value,
        cancellationToken: cancellationToken))
    {
        // ...
    }
}
```

## Related

- [Tariff codes and GSP](tariff-codes.md) — parse and compose tariff codes
- [Products catalogue](products.md) — catalogue snapshots and `TryGetTariff`
- [Consumption](consumption.md) — smart-meter intervals
- [Units, VAT, and time](../explanation/units-vat-and-time.md) — p/kWh, VAT fields, BST, Agile 16:00
- [Pagination](pagination.md)
- [API coverage](../reference/api-coverage.md)
