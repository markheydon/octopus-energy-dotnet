# Electricity and gas consumption

Fetch smart-meter consumption intervals for electricity (MPAN) or gas (MPRN) meter points.

## When to use this guide

You have a dashboard API key and know the MPAN or MPRN and meter serial number for the meter you want to read. Fetch [account detail](accounts.md) first when you need to discover meter points, agreements, and serial numbers from an account number.

Non-smart meters may return an empty list. That is valid API behaviour, not an error.

## Prerequisites

- Dashboard API key — see [authentication](authentication.md)
- MPAN or MPRN and meter serial number (from account detail or your own records)

## List electricity consumption

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Consumption;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

await foreach (ConsumptionInterval interval in client.Consumption.ListElectricityAsync(
    "1000000000001",
    "1111111111",
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"{interval.IntervalStart}: {interval.Consumption} {interval.ConsumptionUnits}");
}
```

Pagination is automatic. Without `period_from` and `period_to` filters the SDK may follow many `next` links. Bound the query when you only need a window:

```csharp
ConsumptionListRequest request = new()
{
    PeriodFrom = new DateTimeOffset(2024, 3, 31, 0, 0, 0, TimeSpan.Zero),
    PeriodTo = new DateTimeOffset(2024, 4, 1, 0, 0, 0, TimeSpan.Zero),
    PageSize = 500,
    OrderBy = ConsumptionOrderBy.PeriodAscending,
    GroupBy = ConsumptionGroupBy.Day,
};

await foreach (ConsumptionInterval interval in client.Consumption.ListElectricityAsync(
    "1000000000001",
    "1111111111",
    request,
    cancellationToken))
{
    // ...
}
```

`PageSize` must be between 1 and `RestPageSizeLimits.ConsumptionMaximum` (25,000). Invalid values throw `OctopusEnergyRequestException` before any HTTP call.

## List gas consumption

Gas uses the same request type and pagination behaviour on `ListGasAsync`:

```csharp
await foreach (ConsumptionInterval interval in client.Consumption.ListGasAsync(
    "1234567890123",
    "2222222222",
    request,
    cancellationToken))
{
    Console.WriteLine($"{interval.IntervalStart}: {interval.Consumption} {interval.ConsumptionUnits}");
}
```

SMETS1 gas meters report **kWh**; SMETS2 meters report **m³**. Check `ConsumptionUnits` and `GasUnit` on each interval rather than assuming a single unit.

## Use meter points from account detail

When you already have an `ElectricityMeterPoint` or `GasMeterPoint` from account detail, pass it instead of raw identifiers:

```csharp
using OctopusEnergy.Client.Models.Accounts;

ElectricityMeterPoint point = account.Properties[0].ElectricityMeterPoints[0];
string meterSerial = point.Meters[0].SerialNumber;

await foreach (ConsumptionInterval interval in client.Consumption.ListElectricityAsync(
    point,
    meterSerial,
    request,
    cancellationToken))
{
    // ...
}
```

Export MPANs still use the `consumption` field name on the wire.

## Single-page listing

When you need the total `count` or one page of intervals without auto-following `next`, use `ListElectricityPageAsync` or `ListGasPageAsync`. These return `PaginatedResult<ConsumptionInterval>` with `Count`, `Results`, and pagination metadata.

```csharp
using OctopusEnergy.Client.Models.Common;

PaginatedResult<ConsumptionInterval> page = await client.Consumption.ListElectricityPageAsync(
    "1000000000001",
    "1111111111",
    request,
    cancellationToken);

Console.WriteLine($"Total count: {page.Count}");
foreach (ConsumptionInterval interval in page.Results)
{
    // ...
}
```

For multi-page iteration, use `ListElectricityAsync` / `ListGasAsync`, or bound the query with `PeriodFrom` and `PeriodTo`. See [pagination](pagination.md).

## Units and time

Electricity consumption is in **kWh** with 0.001 precision. Intervals use ISO 8601 datetimes with `Z` or an explicit offset; the offset may switch around BST transitions.

`GroupBy.Day` groups on **local midnight Europe/London**, not UTC. When you need to construct UK civil times for query parameters, use `OctopusEnergyTime.AssumeEuropeLondon`.

See [units, VAT, and time](../explanation/units-vat-and-time.md) for BST behaviour, Agile 16:00, and billing rounding.

## Join consumption to unit rates

To align consumption intervals with UTC Agile unit-rate periods, fetch rates for the same window and join by instant:

```csharp
using OctopusEnergy.Client.Models.Products;

List<ConsumptionInterval> intervals = [];
await foreach (ConsumptionInterval interval in client.Consumption.ListElectricityAsync(
    mpan, meterSerial, request, cancellationToken))
{
    intervals.Add(interval);
}

TariffCode tariffCode = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");
List<TariffCharge> rates = [];
await foreach (TariffCharge rate in client.TariffRates.ListStandardUnitRatesAsync(
    tariffCode, request, cancellationToken))
{
    rates.Add(rate);
}

IReadOnlyList<(ConsumptionInterval Interval, TariffCharge Rate)> matches =
    ConsumptionPricePeriodMatching.JoinByInstant(intervals, rates);
```

`ConsumptionPricePeriodMatching` finds the rate whose `[ValidFrom, ValidTo)` window contains the interval start instant. It is a join helper, not a cost calculator. See [units, VAT, and time](../explanation/units-vat-and-time.md) and [tariff rates](tariff-rates.md).

## Errors

| HTTP status | Exception | Typical cause |
|---|---|---|
| 401 | `OctopusEnergyApiException` | Missing or invalid API key |
| 403 | `OctopusEnergyApiException` | API access not enabled for the account user |
| 404 | `OctopusEnergyApiException` | Unknown MPAN, MPRN, or meter serial |

Empty or whitespace identifiers throw `OctopusEnergyRequestException` before any HTTP call. Invalid `PageSize` values throw `OctopusEnergyRequestException` before any HTTP call.

See [error handling](error-handling.md).

## Related

- [Account detail](accounts.md) — discover MPANs, MPRNs, and meter serials
- [Tariff rates](tariff-rates.md) — standing charges and unit-rate history
- [Pagination](pagination.md) — automatic vs single-page listing
- [Units, VAT, and time](../explanation/units-vat-and-time.md) — kWh, BST, and rate matching
- [Authentication](authentication.md)
- [API coverage](../reference/api-coverage.md)
