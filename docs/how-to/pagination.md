# Pagination

REST list endpoints return `count`, `next`, `previous`, and `results`. Resource services use the internal REST client to follow `next` until it is null, so callers receive a single `IAsyncEnumerable<T>` without managing page URLs.

Documented `page_size` defaults and maxima are exposed on `RestPageSizeLimits`:

- Default: `100`
- Unit rates and standing charges: maximum `1,500`
- Consumption: maximum `25,000`

Passing a larger `page_size` throws `OctopusEnergyRequestException` before any HTTP call.

## Supplying your own `HttpClient`

When you pass an `HttpClient` to `OctopusEnergyClient`:

- If `BaseAddress` is null, the client sets it to the default UK API URL on the instance you supply.
- If no `Accept: application/json` header is present, the client adds one.

Prefer a dedicated `HttpClient` per client instance, or register via `IHttpClientFactory`, rather than sharing one instance across unrelated callers.

GraphQL (v2) uses Relay cursors with `first` ≤ 100.

See [coding notes](../planning/coding-notes.md).
