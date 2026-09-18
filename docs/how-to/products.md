# Products catalogue

List energy products and load product detail with tariffs by UK distribution region (GSP) and payment method. Public catalogue endpoints do not require an API key.

## List products

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;

using var client = new OctopusEnergyClient();

await foreach (Product product in client.Products.ListAsync(cancellationToken: cancellationToken))
{
    Console.WriteLine($"{product.Code}: {product.DisplayName} ({product.Brand})");
}
```

Pagination is automatic. The SDK follows REST `next` links until all pages are returned.

## Filter the catalogue

Pass a `ProductListRequest` to apply documented filters. Unset properties are omitted from the query string. Do not assume `OCTOPUS_ENERGY` is the only brand on the UK host.

```csharp
ProductListRequest request = new()
{
    Brand = "OCTOPUS_ENERGY",
    IsVariable = true,
    IsGreen = true,
    AvailableAt = new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
};

await foreach (Product product in client.Products.ListAsync(request, cancellationToken))
{
    // ...
}
```

Supported filters: `Brand`, `IsVariable`, `IsGreen`, `IsTracker`, `IsPrepay`, `IsBusiness`, `AvailableAt`. Setting `Brand` to empty or whitespace throws `OctopusEnergyRequestException` before any HTTP call.

## Get product detail

```csharp
ProductDetail agile = await client.Products.GetAsync(
    "AGILE-FLEX-22-11-25",
    cancellationToken: cancellationToken);

Console.WriteLine(agile.TariffsActiveAt);
```

Tariff maps are keyed by `GridSupplyPoint` (parsed from `_A` … `_P` JSON keys):

```csharp
ProductPaymentMethodTariffs london = agile.SingleRegisterElectricityTariffs[GridSupplyPoint.C];
ProductTariff tariff = london.DirectDebitMonthly!;
Console.WriteLine(tariff.Code);
Console.WriteLine(tariff.StandardUnitRateIncVat);
```

Empty payment-method objects in the API JSON (for example `{}` for `direct_debit_quarterly`) deserialise as `null`.

## Historical tariff snapshots

Pass `tariffsActiveAt` to see tariffs active at a past instant:

```csharp
ProductDetail historic = await client.Products.GetAsync(
    "AGILE-FLEX-22-11-25",
    new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
    cancellationToken);
```

## Standing charges and unit-rate history

Product detail includes hypermedia `links` on each tariff (for example `standing_charges`, `standard_unit_rates`). Typed methods to fetch rate history are tracked on issue [#11](https://github.com/markheydon/octopus-energy-dotnet/issues/11). Until then, parse a tariff code and build a relative path with [tariff codes and GSP](tariff-codes.md).

## Related

- [Authentication](authentication.md) — API keys for account and consumption calls
- [Pagination](pagination.md) — how list enumeration works
- [Tariff codes and GSP](tariff-codes.md) — parse tariff codes from product detail
- [API coverage](../reference/api-coverage.md)
