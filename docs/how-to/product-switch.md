# Product switch

Initiate a tariff product switch through GraphQL using the same dashboard API key as REST.

## When to use this guide

You hold a valid quoted product id or commercial offer identifier from Octopus quoting and want to accept it without authoring the `initiateProductSwitch` mutation yourself.

This is a high-impact account change. Confirm dates, product choice, and permissions before calling the live API.

## Prerequisites

- NuGet **1.2.x** or a build from `main` (not package **1.1.0**) — [API coverage](../reference/api-coverage.md)
- Dashboard API key with **Can switch product** or **Can manage account renewals** — see [authentication](authentication.md)
- Account number — see [accounts](accounts.md) or `client.Viewer`
- Exactly one of:
  - `quotedProductId` from a supply-point quote, or
  - `offerIdentifier` for a commercial offer
- Effective switch date as `DateOnly`

The SDK does not create quotes or offers (partner quote REST is out of scope). Obtain identifiers from Octopus flows outside this package.

## Initiate a switch

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.ProductSwitch;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

ProductSwitchInitiate request = new()
{
    AccountNumber = "A-12345678",
    SwitchDate = new DateOnly(2026, 4, 1),
    QuotedProductId = "your-quoted-product-id",
};

ProductSwitchResult result = await client.ProductSwitch.InitiateAsync(
    request,
    cancellationToken);

Console.WriteLine($"Switched to {result.ProductCode} effective {result.SwitchDate}");
```

To accept an offer instead of a quoted product id, set `OfferIdentifier` and omit `QuotedProductId`.

## Interpreting the product code

Where `result.ProductCode` matches the UK tariff encoding used on REST, parse it with `TariffCode.TryParse` — see [tariff codes](tariff-codes.md).

## Errors

GraphQL failures surface as `OctopusEnergyKrakenException` with `KT-CT-*` codes (for example invalid quote, switch date out of range, or unauthorised key). See [error handling](error-handling.md).
