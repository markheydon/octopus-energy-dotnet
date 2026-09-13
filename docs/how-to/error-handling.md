# Error handling

Not implemented.

REST will use HTTP status codes. GraphQL (v2) usually returns HTTP 200 with `errors.extensions.errorCode` values such as `KT-CT-1112`.

Public types will live under `OctopusEnergy.Client` / infrastructure exceptions, analogous to FreeAgent.NET.
