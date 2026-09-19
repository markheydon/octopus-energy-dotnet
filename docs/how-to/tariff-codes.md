# Tariff codes and GSP helpers

Octopus tariff codes encode fuel, register count, product code, and UK distribution region (GSP). Product detail JSON keys regions as `_A` … `_P`. The SDK exposes typed helpers so callers do not concatenate URL segments by hand.

## Parse a tariff code

```csharp
using OctopusEnergy.Client;

TariffCode tariff = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");

Console.WriteLine(tariff.ProductCode);      // AGILE-FLEX-22-11-25
Console.WriteLine(tariff.GridSupplyPoint);  // C (London)
Console.WriteLine(tariff.ToString());       // E-1R-AGILE-FLEX-22-11-25-C
```

Invalid codes throw `OctopusEnergyRequestException` before any HTTP call.

## Map `_C` and `C`

Industry lookup and product JSON use underscore-prefixed group ids. Tariff codes use a single letter suffix.

```csharp
GridSupplyPoint london = GridSupplyPointParser.Parse("_C");  // also accepts "C"
string groupId = GridSupplyPointParser.ToGroupId(london);      // "_C"
```

`GridSupplyPoint` serialises as `_C` in JSON when used on models.

## List standing charges and unit rates

Use `client.TariffRates` rather than building REST paths manually. `TariffCode.GetRelativeChargePath` is obsolete and kept for compatibility only.

```csharp
TariffCode agile = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");

await foreach (TariffCharge rate in client.TariffRates.ListStandardUnitRatesAsync(
    agile,
    cancellationToken: cancellationToken))
{
    Console.WriteLine($"{rate.ValidFrom}: {rate.ValueIncVat} p/kWh");
}

TariffCode economy7 = TariffCode.Parse("E-2R-VAR-22-11-01-A");
await foreach (TariffCharge dayRate in client.TariffRates.ListDayUnitRatesAsync(
    economy7,
    cancellationToken: cancellationToken))
{
    // ...
}
```

Gas tariffs support standing charges and standard unit rates only. `ListDayUnitRatesAsync` and `ListNightUnitRatesAsync` throw `OctopusEnergyRequestException` for gas or single-register electricity.

## Compose a code without parsing

```csharp
var tariff = new TariffCode(
    EnergyFuel.Electricity,
    TariffRegisterKind.SingleRegister,
    "AGILE-FLEX-22-11-25",
    GridSupplyPoint.C);
```

See [coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (section 8) for tariff encoding background. Product detail is available via [products](products.md). Use `client.TariffRates` for standing charges and unit-rate history.
