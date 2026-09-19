# Industry lookups

Resolve a UK distribution region (GSP) from a postcode, or look up an electricity meter point by MPAN. These are public endpoints and do not require an API key.

## When to use this guide

You need a `GridSupplyPoint` before loading product detail tariffs for a region, or you want to confirm the GSP and profile class for an MPAN without fetching a full account.

## Prerequisites

- No API key required
- A UK postcode or a 13-digit MPAN

## GSP by postcode

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Industry;

CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient();

await foreach (GridSupplyPointLookup lookup in client.Industry.ListGridSupplyPointsByPostcodeAsync(
    "W1 1AA",
    cancellationToken))
{
    Console.WriteLine($"{lookup.Postcode}: GSP {lookup.GridSupplyPoint}");
}
```

A postcode may return multiple rows when several MPANs map to different regions. Use `GridSupplyPoint` on each result when calling [products catalogue](products.md) or parsing tariff codes.

Leading and trailing whitespace on the postcode is trimmed. Null or whitespace postcodes throw `OctopusEnergyRequestException` before any HTTP call.

## MPAN lookup

```csharp
ElectricityMeterPointLookup meterPoint = await client.Industry.GetElectricityMeterPointAsync(
    "1000000000001",
    cancellationToken);

Console.WriteLine($"GSP: {meterPoint.GridSupplyPoint}");
Console.WriteLine($"Profile class: {meterPoint.ProfileClass}");
```

This returns the GSP and profile class for the MPAN. It does not return consumption, agreements, or account details. For those, use [account detail](accounts.md) with an API key.

## Map GSP to tariff codes

Industry lookup and product JSON use underscore-prefixed group ids (`_C`). Tariff codes use a single letter suffix (`C`). See [tariff codes and GSP](tariff-codes.md) for `GridSupplyPointParser` helpers.

```csharp
GridSupplyPoint london = GridSupplyPointParser.Parse("_C");
string groupId = GridSupplyPointParser.ToGroupId(london);  // "_C"
```

## Errors

| HTTP status | Exception | Typical cause |
|---|---|---|
| 404 | `OctopusEnergyApiException` | Unknown postcode or MPAN |

Empty or whitespace MPAN values throw `OctopusEnergyRequestException` before any HTTP call.

See [error handling](error-handling.md).

## Related

- [Tariff codes and GSP](tariff-codes.md) — parse tariff codes and map `_C` ↔ `C`
- [Products catalogue](products.md) — load tariffs by GSP
- [Account detail](accounts.md) — full account, meter, and agreement data
- [API coverage](../reference/api-coverage.md)
