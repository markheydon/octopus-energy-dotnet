# REST and GraphQL

Today's package is **REST only** (milestones **v1.0** and **v1.1**, shipped on NuGet **1.0.x** and **1.1.x**). **GraphQL customer extras** are planned for milestone **v1.2** (NuGet **1.2.x**, not package **2.0.0**). See [release labels](../../VERSIONING.md#release-labels-quick-reference).

Callers should still construct a single `OctopusEnergyClient` with an API key. When GraphQL ships, the SDK may exchange that key for a JWT internally — you will not choose REST versus GraphQL per call.

Do not expose GraphQL query strings or REST URL templates on the public surface. See [adr-0001](../../adr/adr-0001-customer-sdk-dual-transport.md).
