# Customer vs partner APIs

Octopus documents one GraphQL schema and one REST host for “customers and partner organisations”. That wording oversells partner capabilities to library authors.

**Customers** generate an API key in the dashboard. They can list public products, read their account, and pull smart-meter consumption. GraphQL adds bills, devices, Mini telemetry, and similar *if* the field is enabled for that viewer.

**Partners** receive organisation credentials and can create quotes, enrol accounts, import data, and use Hub-configured limits.

This SDK is customer-only. See [SCOPE.md](../../SCOPE.md) and [coding notes](../planning/coding-notes.md).
