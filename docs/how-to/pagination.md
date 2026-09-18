# Pagination

REST list endpoints return `count`, `next`, `previous`, and `results`. Resource services follow `next` until it is null, so callers receive a single `IAsyncEnumerable<T>` without managing page URLs.

## Automatic pagination

```csharp
await foreach (Product product in client.Products.ListAsync(cancellationToken))
{
    // Each item; the SDK fetches further pages as needed.
}
```

The same pattern applies to consumption intervals, tariff rate history, and other list resources.

## Documented `page_size` limits

Official defaults and maxima are exposed on `RestPageSizeLimits` — use these constants rather than hard-coding numbers:

| Constant | Value | Use |
|---|---|---|
| `RestPageSizeLimits.Default` | `100` | Default when callers omit `page_size` |
| `RestPageSizeLimits.RatesMaximum` | `1,500` | Unit rates and standing charges |
| `RestPageSizeLimits.ConsumptionMaximum` | `25,000` | Electricity and gas consumption |

Passing a larger `page_size` throws `OctopusEnergyRequestException` before any HTTP call.

Implementer source: [coding notes](../planning/coding-notes.md) (§6).

## GraphQL (v2)

v2 GraphQL list operations will use Relay cursors with `first` ≤ 100. v1 REST callers do not configure this.

## Supplying your own `HttpClient`

When you pass an `HttpClient` to `OctopusEnergyClient`:

- If `BaseAddress` is null, the client sets it to the default UK API URL on the instance you supply.
- If `BaseAddress` is already set, a trailing slash is appended when missing so relative REST paths resolve correctly.
- If no `Accept: application/json` header is present, the client adds one.
- When you use an API-key constructor, HTTP Basic authentication is applied and any existing `Authorization` header is replaced.

Prefer a dedicated `HttpClient` per client instance, or register via `IHttpClientFactory`, rather than sharing one instance across unrelated callers.

See [authentication](authentication.md) for API-key handling and secret hygiene.

## Related

- [Error handling](error-handling.md) — pagination stops on non-success HTTP responses
- [Products catalogue](products.md)
