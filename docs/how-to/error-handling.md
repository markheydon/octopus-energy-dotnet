# Error handling

Public exception types live in the `OctopusEnergy.Client` namespace:

| Type | When it is thrown |
|---|---|
| `OctopusEnergyException` | Base type for SDK failures |
| `OctopusEnergyRequestException` | Local contract violations (for example `page_size` above a documented maximum, or an invalid tariff code or GSP) before HTTP |
| `OctopusEnergyHttpException` | Non-success HTTP status without a documented API error payload |
| `OctopusEnergyApiException` | Non-success HTTP with a documented REST `detail` message |
| `OctopusEnergyParseException` | Successful HTTP response that cannot be deserialised (invalid JSON or JSON `null` where a model is expected) |

REST uses HTTP status codes. GraphQL (v2) usually returns HTTP 200 with `errors.extensions.errorCode` values such as `KT-CT-1112`; those will map to the same hierarchy in a later release.

## Catch SDK failures

```csharp
CancellationToken cancellationToken = default;

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
catch (OctopusEnergyParseException ex)
{
    // HTTP 2xx but the body could not be deserialised.
    Console.WriteLine(ex.Message);
}
```

Catch `OctopusEnergyException` when you do not need to distinguish subtypes.

## Rate limiting and retries

By default, the SDK retries transient HTTP `429 Too Many Requests` and `503 Service Unavailable` responses up to three times after the first failure (four HTTP calls in total). When the API sends a `Retry-After` header, the SDK waits for that duration (capped at 60 seconds) before retrying. When `Retry-After` is absent, the SDK uses exponential backoff from a one-second base delay, also capped at 60 seconds.

Retries honour `CancellationToken` cancellation. Other status codes (including `401`, `403`, `404`, and `500`) are not retried.

To disable SDK retry and fail fast:

```csharp
using OctopusEnergy.Client;

using HttpClient httpClient = httpClientFactory.CreateClient("OctopusEnergy");
using OctopusEnergyClient client = new(httpClient, OctopusEnergyRetryOptions.Disabled);
```

Custom limits:

```csharp
OctopusEnergyRetryOptions retryOptions = new()
{
    MaxAttempts = 1,
    MaxDelay = TimeSpan.FromSeconds(10),
    BaseDelay = TimeSpan.FromMilliseconds(500),
};

using OctopusEnergyClient client = new(httpClient, retryOptions);
```

If your host already retries HTTP 429 or 503 (for example with Polly on the `HttpClient` pipeline), disable one layer to avoid double backoff.

## Pagination and errors

Pagination stops and throws on non-success HTTP responses. Items from pages that were already yielded remain available; the exception is raised when the failing page is requested. A truncated page is never returned after an error. Each paginated page request has its own retry budget.

## Related

- [Pagination](pagination.md)
- [Authentication](authentication.md) - protect API keys; logging middleware may capture `Authorization`
