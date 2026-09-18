# OctopusEnergy.Client

Unofficial .NET client for the **Octopus Energy customer APIs** (public REST catalogue plus authenticated account and consumption data). Not affiliated with Octopus Energy Limited.

Dashboard API keys expose account structure and consumption. Treat keys as secrets and do not share them with untrusted third parties. This library is not a developer licence from Octopus Energy.

This package is **prerelease**. The public API will change. See the [repository README](https://github.com/markheydon/octopus-energy-dotnet) and [VERSIONING.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/VERSIONING.md).

Full consumer documentation: [markheydon.me.uk/octopus-energy-dotnet](https://markheydon.me.uk/octopus-energy-dotnet/).

## Units

- Electricity consumption: **kWh** at **0.001** precision.
- Gas: **SMETS1** reports kWh; **SMETS2** reports **m³**.
- Tariff rates: **p/kWh** and **p/day**. Quotes and bills often use **pence**.

Details: [units, VAT, and time](https://markheydon.me.uk/octopus-energy-dotnet/explanation/units-vat-and-time/).

## VAT

Many fields appear as `*ExcVat` and `*IncVat` (excluding and including VAT). Domestic supply VAT is typically **5%**; the SDK documents that rate and does not encode tax advice.

## Timezones

Octopus treats datetimes without an offset as **Europe/London**. The SDK uses
`DateTimeOffset` on all REST models and serialises query parameters with `Z` or an
explicit offset.

Consumption intervals may switch between `Z` and `+01:00` around BST transitions.
Agile unit rates stay UTC. Join rates to consumption by instant
(`DateTimeOffset`), not by UTC date or `DateTimeKind.Unspecified`.

Agile day-ahead rates typically publish by **16:00 Europe/London**; a short day (46 vs 48 half-hours) before then is normal.

Use `OctopusEnergyTime.AssumeEuropeLondon(...)` when you need to construct a UK
civil time that the API would treat as local.

Details: [units, VAT, and time](https://markheydon.me.uk/octopus-energy-dotnet/explanation/units-vat-and-time/).
