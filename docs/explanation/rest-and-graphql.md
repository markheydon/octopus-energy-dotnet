# REST and GraphQL

Most of the shipped package remains **REST** (milestones **v1.0** and **v1.1** on NuGet **1.0.x** and **1.1.x**). **GraphQL customer extras** expand in milestone **v1.2** (NuGet **1.2.x**, not package **2.0.0**). See [release labels](../../VERSIONING.md#release-labels-quick-reference).

Callers should still construct a single `OctopusEnergyClient` with an API key. The SDK exchanges that key for a Kraken JWT internally when a GraphQL-backed call needs it — you do not choose REST versus GraphQL per call, and you do not attach GraphQL tokens yourself.

The first public GraphQL-backed capabilities are **`client.Viewer`** (discover account numbers via `viewer`), **`client.RateLimits`** (hourly complexity points allowance and remaining balance via `rateLimitInfo`), **`client.MeterReadings`** (electricity and gas reading submission plus `readingConsentGranularity` lookup), **`client.AccountBilling`** (Relay lists for bills, transactions, and payments on `account`), **`client.Devices`** (SmartFlex device lists and `flexPlannedDispatches`), **`client.SmartMeterTelemetry`** (EUI64 discovery and `smartMeterTelemetry`), **`client.Octoplus`** (enrolment, loyalty points, wheel-of-fortune allowance, and related mutations on the public host), and **`client.ProductSwitch`** (`initiateProductSwitch` when the key is permitted). Saving-session join APIs are not on the public GraphQL schema. Constants for Relay paging and operational limits live on `GraphQlRelayLimits`; see [pagination](../how-to/pagination.md).

Do not expose GraphQL query strings or REST URL templates on the public surface. See [adr-0001](../../adr/adr-0001-customer-sdk-dual-transport.md).
