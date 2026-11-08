# Pagination

REST list endpoints return `count`, `next`, `previous`, and `results`. Resource services follow `next` until it is null, so callers receive a single `IAsyncEnumerable<T>` without managing page URLs.

## Automatic pagination

```csharp
await foreach (Product product in client.Products.ListAsync())
{
    // Each item; the SDK fetches further pages as needed.
}
```

The same pattern applies to consumption intervals, tariff rate history, and other list resources.

## Single-page consumption listing

When you need the total `count` or one page of intervals without auto-following `next`, use `ListElectricityPageAsync` or `ListGasPageAsync` on `client.Consumption`. These return `PaginatedResult<ConsumptionInterval>` with `Count`, `Results`, and pagination metadata. See [consumption](consumption.md) for filters, units, and meter-point overloads.

`PaginatedResult.Next` is informational. The SDK does not expose a public method to fetch that URL. For multi-page iteration, use `ListElectricityAsync` / `ListGasAsync`, or bound the query with `period_from` and `period_to` on `ConsumptionListRequest`.

## Documented `page_size` limits

Official defaults and maxima are exposed on `RestPageSizeLimits` - use these constants rather than hard-coding numbers:

| Constant | Value | Use |
|---|---|---|
| `RestPageSizeLimits.Default` | `100` | Default when callers omit `page_size` |
| `RestPageSizeLimits.RatesMaximum` | `1,500` | Unit rates and standing charges |
| `RestPageSizeLimits.ConsumptionMaximum` | `25,000` | Electricity and gas consumption |

Passing `page_size` less than 1 or above the documented maximum throws `OctopusEnergyRequestException` before any HTTP call. This applies to consumption and tariff rate list requests.

Implementer source: [coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (section 6).

## GraphQL Relay limits (milestone v1.2)

GraphQL list operations use Relay cursors. Official limits are on `GraphQlRelayLimits`:

| Constant | Value | Meaning |
|---|---|---|
| `GraphQlRelayLimits.MaxFirst` | `100` | Maximum `first` on a connection |
| `GraphQlRelayLimits.MaxComplexityPerRequest` | `200` | Per-request complexity cap (`KT-CT-1188`) |
| `GraphQlRelayLimits.DefaultHourlyPointsAllowance` | `50,000` | Default hourly points for dashboard account users |
| `GraphQlRelayLimits.MaxNodesPerRequest` | `10,000` | Node cap per request (`KT-CT-1189`) |

Calling `GraphQlRelayLimits.ValidateFirst` with `first` less than 1 or above `MaxFirst` throws `OctopusEnergyRequestException` before any HTTP call. `client.AccountBilling` applies this on `ListBillsPageAsync`, `ListTransactionsPageAsync`, and `ListPaymentsPageAsync` via each request's `First` value.

Relay lists return one page at a time (`GraphQlRelayPage<T>` with `PageInfo.HasNextPage` and `EndCursor`). Loop with the `after` cursor until `HasNextPage` is false. See [account billing](account-billing.md).

When you page forward with an `after` cursor, Kraken reports `hasPreviousPage` as always false. Use `GraphQlRelayPageInfo.ApplyForwardPagingRules` so you do not trust a `true` value while forward-paging.

Inspect remaining hourly points with `client.RateLimits.GetRateLimitInfoAsync` (requires a dashboard API key). See [error handling](error-handling.md) for Kraken error codes.

Implementer source: [coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (section 6) and [ADR-0002](https://github.com/markheydon/octopus-energy-dotnet/blob/main/adr/adr-0002-graphql-public-list-patterns.md).

## Supplying your own `HttpClient`

When you pass an `HttpClient` to `OctopusEnergyClient`:

- If `BaseAddress` is null, the SDK uses the default UK API URL internally for relative paths without mutating the supplied instance.
- If `BaseAddress` is already set, a trailing slash is normalised when resolving relative REST paths.
- `Accept: application/json`, `User-Agent`, and HTTP Basic authentication (when an API key is supplied) are applied per request, not on `DefaultRequestHeaders`.

Prefer resolving clients from `IHttpClientFactory` in hosted applications, or a dedicated `HttpClient` per SDK client instance in console tools.

See [authentication](authentication.md) for API-key handling and secret hygiene.

## Related

- [Error handling](error-handling.md) - pagination stops on non-success HTTP responses
- [Products catalogue](products.md)
