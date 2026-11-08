# Octoplus enrolment, points, and wheel of fortune

Optional GraphQL helpers for Octoplus enrolment, loyalty points, and wheel-of-fortune spin allowance. They use the same dashboard API key as REST.

## When to use this guide

You want Octoplus status or points without hand-authoring GraphQL. Saving-session **join** flows are not on the public Kraken GraphQL host (`https://api.octopus.energy/v1/graphql/`); they remain outside this SDK per [SCOPE.md](../../SCOPE.md).

## Prerequisites

- NuGet **1.2.x** or a build from `main` (not package **1.1.0**) — [API coverage](../reference/api-coverage.md)
- Dashboard API key — see [authentication](authentication.md)
- Account number — see [accounts](accounts.md) or `client.Viewer`
- Octoplus enrolment for balance and spin queries (otherwise expect `KT-CT-1111` unauthorised errors)

## Enrolment status

```csharp
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Octoplus;

const string apiKey = "sk_test_not_a_real_key";
CancellationToken cancellationToken = default;

using var client = new OctopusEnergyClient(apiKey);

OctoplusAccountInfo info = await client.Octoplus.GetAccountInfoAsync(
    "A-12345678",
    cancellationToken);

Console.WriteLine($"{info.EnrollmentStatus} loyalty={info.IsLoyaltyPointsUser}");
```

## Loyalty points

```csharp
LoyaltyPointsBalance balance = await client.Octoplus.GetLoyaltyPointsBalanceAsync(
    "A-12345678",
    cancellationToken: cancellationToken);

IReadOnlyList<LoyaltyPointLedgerEntry> ledgers = await client.Octoplus.ListLoyaltyPointLedgersAsync(
    cancellationToken);
```

`ListLoyaltyPointLedgersAsync` issues one GraphQL query. The public schema exposes `loyaltyPointLedgers` as a list field, not a Relay connection, so the SDK returns the full response in a single call. There is no `first`/`after` paging on this path; if Kraken adds connection-style pagination later, the SDK would need a follow-up change.

## Wheel of fortune (spin allowance)

The underlying GraphQL field is deprecated but still returned for eligible accounts:

```csharp
WheelOfFortuneSpinsAvailability spins = await client.Octoplus.GetWheelOfFortuneSpinsAsync(
    "A-12345678",
    cancellationToken);

Console.WriteLine($"Elec remaining: {spins.Electricity?.RemainingSpinsThisMonth}");
```

## Join and redeem

```csharp
JoinOctoplusCampaignResult joined = await client.Octoplus.JoinCampaignAsync(
    "A-12345678",
    cancellationToken);

RedeemLoyaltyPointsResult redeemed = await client.Octoplus.RedeemPointsForAccountCreditAsync(
    "A-12345678",
    points: 100,
    cancellationToken: cancellationToken);
```

Mutations surface Kraken business and permission errors as `OctopusEnergyKrakenException`; the SDK does not interpret enrolment rules locally.

## Further reading

- [GraphQL customer allow-list](../planning/graphql-customer-allow-list.md) — runtime probe matrix
- [API coverage](../reference/api-coverage.md)
