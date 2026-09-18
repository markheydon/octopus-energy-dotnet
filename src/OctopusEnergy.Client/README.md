# OctopusEnergy.Client

Unofficial .NET client for the **Octopus Energy customer APIs** (public REST catalogue plus authenticated account and consumption data). Not affiliated with Octopus Energy Limited.

This package is **prerelease**. The public API will change. See the [repository README](https://github.com/markheydon/octopus-energy-dotnet) and [VERSIONING.md](https://github.com/markheydon/octopus-energy-dotnet/blob/main/VERSIONING.md).

## Timezones

Octopus treats datetimes without an offset as **Europe/London**. The SDK uses
`DateTimeOffset` on all REST models and serialises query parameters with `Z` or an
explicit offset.

Consumption intervals may switch between `Z` and `+01:00` around BST transitions.
Agile unit rates stay UTC. Join rates to consumption by instant
(`DateTimeOffset`), not by UTC date or `DateTimeKind.Unspecified`.

Use `OctopusEnergyTime.AssumeEuropeLondon(...)` when you need to construct a UK
civil time that the API would treat as local.
