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

Tariff maps are keyed by `GridSupplyPoint` (parsed from `_A` … `_P` JSON keys). Use `TryGetTariff` to resolve a catalogue tariff by fuel, register kind, region, and payment method:

```csharp
if (agile.TryGetTariff(
        EnergyFuel.Electricity,
        TariffRegisterKind.SingleRegister,
        GridSupplyPoint.C,
        ProductPaymentMethod.DirectDebitMonthly,
        out ProductTariff? tariff))
{
    Console.WriteLine(tariff!.Code);
    Console.WriteLine(tariff.StandardUnitRateIncVat);
    Console.WriteLine(tariff.ParsedTariffCode?.ProductCode);
}
```

When you already have a parsed `TariffCode` (for example from `TariffAgreement.ParsedTariffCode`), pass it directly. The helper checks that the product code matches this `ProductDetail` and that the returned tariff code matches:

```csharp
TariffCode? agreementCode = importPoint.Agreements[0].ParsedTariffCode;
if (agreementCode is not null
    && agile.TryGetTariff(agreementCode.Value, ProductPaymentMethod.DirectDebitMonthly, out ProductTariff? matched))
{
    Console.WriteLine(matched!.Code);
}
```

You can still index `SingleRegisterElectricityTariffs[GridSupplyPoint.C].DirectDebitMonthly` when you prefer direct dictionary access.

Empty payment-method objects in the API JSON (for example `{}` for `direct_debit_quarterly`) deserialise as `null`; `TryGetTariff` returns `false` for absent payment methods.

## Historical tariff snapshots

Pass `tariffsActiveAt` to see tariffs active at a past instant:

```csharp
ProductDetail historic = await client.Products.GetAsync(
    "AGILE-FLEX-22-11-25",
    new DateTimeOffset(2019, 1, 1, 0, 0, 0, TimeSpan.Zero),
    cancellationToken);
```

## Standing charges and unit-rate history

Product detail includes hypermedia `links` on each tariff (for example `standing_charges`, `standard_unit_rates`). Use `client.TariffRates` to list standing charges and unit rates for a parsed `TariffCode`. A dedicated how-to for rate history will follow; until then, see [tariff codes and GSP](tariff-codes.md) and [units, VAT, and time](../explanation/units-vat-and-time.md).

## Related

- [Authentication](authentication.md) - API keys for account and consumption calls
- [Pagination](pagination.md) - how list enumeration works
- [Tariff codes and GSP](tariff-codes.md) - parse tariff codes from product detail
- [API coverage](../reference/api-coverage.md)
