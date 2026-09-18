# Authentication

Octopus customer REST calls use a dashboard **API key** sent as HTTP Basic authentication. The key is the username; the password is empty.

## Create an API key

Generate a key in the [Octopus dashboard](https://octopus.energy/dashboard/new/accounts/personal-details/api-access). Treat it as a secret. The SDK never logs API keys or JWTs.

## Construct the client

For account and consumption calls, pass the key at construction time:

```csharp
using OctopusEnergy.Client;

const string apiKey = "sk_test_not_a_real_key";
using var client = new OctopusEnergyClient(apiKey);
```

Public catalogue endpoints (for example products) work without a key. Use the parameterless constructor when you only need unauthenticated calls:

```csharp
using var client = new OctopusEnergyClient();
```

## Custom base URL

The default base URL is `https://api.octopus.energy/v1/`. Other Kraken retail hosts can be supplied for experimentation; the SDK does not claim multi-region support in v1:

```csharp
using var client = new OctopusEnergyClient(
    apiKey,
    new Uri("https://api.example.test/v1/"));
```

A trailing slash is applied when missing so relative REST paths resolve correctly. The same normalisation applies when you supply your own `HttpClient` with a `BaseAddress`.

## Supplying your own `HttpClient`

When you pass an `HttpClient`, the SDK applies HTTP Basic for the API key and replaces any existing `Authorization` header. If `BaseAddress` is already set without a trailing slash, one is appended. See also [pagination](pagination.md#supplying-your-own-httpclient).

```csharp
using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.test/v1/") };
using var client = new OctopusEnergyClient(apiKey, httpClient);
```

## Protect the key in your application

- Do not commit API keys to source control or test fixtures.
- Store keys in environment variables, user secrets, or your host's secret manager.
- Custom `HttpClient` handlers, logging middleware, or diagnostic tools may capture the `Authorization` header. Review anything that logs outbound HTTP requests before enabling it in production.

GraphQL JWT exchange is a v2 implementation detail. v1 REST uses Basic only.

See [coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (section 3, REST).
