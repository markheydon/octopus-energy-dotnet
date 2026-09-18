# Error handling

Public exception types live in the `OctopusEnergy.Client` namespace:

| Type | When it is thrown |
|---|---|
| `OctopusEnergyException` | Base type for SDK failures |
| `OctopusEnergyRequestException` | Local contract violations (for example `page_size` above a documented maximum, or an invalid tariff code or GSP) before HTTP |
| `OctopusEnergyHttpException` | Non-success HTTP status without a documented API error payload |
| `OctopusEnergyApiException` | Non-success HTTP with a documented REST `detail` message |

REST uses HTTP status codes. GraphQL (v2) usually returns HTTP 200 with `errors.extensions.errorCode` values such as `KT-CT-1112`; those will map to the same hierarchy in a later release.

## Catch SDK failures

```csharp
try
{
    Account account = await client.Accounts.GetAsync("A-UNKNOWN", cancellationToken);
}
catch (OctopusEnergyApiException ex)
{
    // REST error body with detail (for example unknown account).
    Console.WriteLine(ex.Message);
}
catch (OctopusEnergyHttpException ex)
{
    // Non-success HTTP without a parseable error payload.
    Console.WriteLine($"{ex.StatusCode}: {ex.Message}");
}
catch (OctopusEnergyRequestException ex)
{
    // Invalid argument or documented limit before any HTTP call.
    Console.WriteLine(ex.Message);
}
```

Catch `OctopusEnergyException` when you do not need to distinguish subtypes.

## Pagination and errors

Pagination stops and throws on non-success HTTP responses. Items from pages that were already yielded remain available; the exception is raised when the failing page is requested. A truncated page is never returned after an error.

## Related

- [Pagination](pagination.md)
- [Authentication](authentication.md) - protect API keys; logging middleware may capture `Authorization`
