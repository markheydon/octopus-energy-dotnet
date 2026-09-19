# Units, VAT, and time

Octopus REST responses mix physical units, money fields, and datetimes that do not all behave the same way. This page explains what callers see so you do not have to rediscover the quirks from upstream docs alone.

The SDK returns typed models and documents official contract limits (for example `page_size` maxima). It does not encode tariff advice, wholesale modelling, or cheapest-slot opinions.

## Units

**Electricity consumption** is reported in **kWh** with **0.001** precision (three decimal places).

**Gas consumption** depends on the meter:

- **SMETS1** meters report **kWh**.
- **SMETS2** meters report **m³**.

Model and display the unit your interval payload uses. Do not assume gas is always kWh.

**Tariff rates** use **p/kWh** for unit rates and **p/day** for standing charges. Product detail exposes snapshot rates on `ProductTariff` (for example `StandardUnitRateIncVat`). Rate history returns `TariffCharge` rows with `ValueIncVat` and `ValueExcVat`.

**Quotes and bills** often use **pence** rather than pounds. Check field names and magnitudes before formatting for display.

**Billing rounding:** Octopus billing rounds consumption **half to even** to **0.01 kWh** before multiplying by price. The SDK does not ship a cost calculator in v1. If you build one, match that rule or label the result as approximate.

## VAT

Many money fields appear twice as excluding- and including-VAT variants:

- Property names ending in `ExcVat` / `IncVat` (for example `StandardUnitRateExcVat` on product detail)
- Wire names vary by endpoint: `standard_unit_rate_exc_vat` on product tariffs; `value_exc_vat` on standing-charge and unit-rate history (`TariffCharge`)

Domestic supply VAT is typically **5%**. The SDK documents that rate; it does not validate your tax treatment or business rules.

Use the field that matches your display or reconciliation need. Do not assume every monetary value is VAT-inclusive.

## Timezones and BST

Octopus treats datetimes **without an offset** as **Europe/London**. The SDK uses `DateTimeOffset` on REST models and serialises query parameters with `Z` or an explicit offset.

**Consumption intervals** may switch between `Z` and `+01:00` around BST transitions. **Agile unit rates stay UTC.** Join rates to consumption by **instant** (`DateTimeOffset`), not by UTC calendar date or `DateTimeKind.Unspecified`.

When you need to construct a UK civil time the API would treat as local, pass wall-clock components with `DateTimeKind.Unspecified` (not `DateTimeKind.Local` from the machine timezone):

```csharp
DateTimeOffset from = OctopusEnergyTime.AssumeEuropeLondon(
    new DateTime(2024, 3, 31, 0, 30, 0, DateTimeKind.Unspecified));
```

Civil times in the spring-forward gap (the skipped hour) throw `OctopusEnergyRequestException`. Ambiguous times during the autumn clock change resolve to standard time (GMT).

`ConsumptionPricePeriodMatching` helps align a consumption interval with a UTC Agile rate period by finding the rate whose `[ValidFrom, ValidTo)` window contains the interval start instant. Consumption may overlap the requested window; the helper does not require `ValidFrom` to equal `IntervalStart`. It is a join helper, not a cost calculator.

`group_by=day` on consumption uses **local midnight**, not UTC.

## Agile and 16:00 Europe/London

**Agile** day-ahead unit rates typically publish by **16:00 Europe/London**. Before then, a **short day** (for example **46** half-hour slots instead of **48**) is normal - do not treat it as missing data.

An Agile **pricing day** follows the **CET market index**, roughly **23:00-23:00 UK** civil time, not a simple UTC or calendar-day boundary.

**Go** tariffs use long `valid_from` / `valid_to` windows rather than 48 half-hour slots. **Economy 7** uses separate day and night rate URLs (`E-2R-…` tariff codes).

This SDK returns documented rates; it does not predict wholesale prices or recommend when to consume.

## Related

- [Getting started](../tutorial/getting-started.md)
- [Pagination](../how-to/pagination.md)
- [Error handling](../how-to/error-handling.md)
- [REST and GraphQL](rest-and-graphql.md) - v1 is REST; GraphQL extras are v2
- [Coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (sections 7-9) - implementer source for numbers and quirks
