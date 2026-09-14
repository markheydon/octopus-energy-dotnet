# Error handling

Public exception types live in the `OctopusEnergy.Client` namespace:

| Type | When it is thrown |
|---|---|
| `OctopusEnergyException` | Base type for SDK failures |
| `OctopusEnergyRequestException` | Local contract violations (for example `page_size` above a documented maximum) before HTTP |
| `OctopusEnergyHttpException` | Non-success HTTP status without a documented API error payload |
| `OctopusEnergyApiException` | Non-success HTTP with a documented REST `detail` message |

REST uses HTTP status codes. GraphQL (v2) usually returns HTTP 200 with `errors.extensions.errorCode` values such as `KT-CT-1112`; those will map to the same hierarchy in a later release.

Pagination stops and throws on non-success HTTP responses; partial result sets are not returned after a failure.
