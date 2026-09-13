# REST and GraphQL

v1 talks to REST only. v2 may add GraphQL internally.

Callers should still construct a single `OctopusEnergyClient` with an API key. The SDK may exchange that key for a JWT when a GraphQL operation runs.

Do not expose GraphQL query strings or REST URL templates on the public surface. See [adr-0001](../../adr/adr-0001-customer-sdk-dual-transport.md).
