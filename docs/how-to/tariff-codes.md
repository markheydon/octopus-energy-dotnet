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

## Build a charge list path

Rate and standing-charge services use relative REST paths. Call `GetRelativeChargePath` on a parsed or constructed `TariffCode`:

```csharp
TariffCode agile = TariffCode.Parse("E-1R-AGILE-FLEX-22-11-25-C");

string ratesPath = agile.GetRelativeChargePath(TariffChargeKind.StandardUnitRates);
// products/AGILE-FLEX-22-11-25/electricity-tariffs/E-1R-AGILE-FLEX-22-11-25-C/standard-unit-rates/

TariffCode economy7 = TariffCode.Parse("E-2R-VAR-22-11-01-A");
string dayPath = economy7.GetRelativeChargePath(TariffChargeKind.DayUnitRates);
```

Gas tariffs support standing charges and standard unit rates only. Day and night unit-rate paths apply to dual-register electricity (`E-2R-…`) and throw `OctopusEnergyRequestException` for gas.

## Compose a code without parsing

```csharp
var tariff = new TariffCode(
    EnergyFuel.Electricity,
    TariffRegisterKind.SingleRegister,
    "AGILE-FLEX-22-11-25",
    GridSupplyPoint.C);
```

See [coding notes](../planning/coding-notes.md) (§8) for tariff encoding background. Product detail is available via [products](products.md). Typed rate-history services are tracked on issue [#11](https://github.com/markheydon/octopus-energy-dotnet/issues/11).
