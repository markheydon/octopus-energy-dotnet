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

A trailing slash is applied when missing so relative REST paths resolve correctly.

## Hosted applications (`IHttpClientFactory`)

In ASP.NET Core or worker hosts, register a named `HttpClient` and add `OctopusEnergyClientHandler` so authentication, `Accept`, and `User-Agent` are applied **per request** without mutating shared `HttpClient` default headers.

The SDK does not reference `Microsoft.Extensions.Http`; add that package in your host project.

Choose **one** place to supply the API key:

- **Handler carries auth** (recommended when the same named client is used outside the SDK), or
- **Client carries auth** (simpler when the named client is only used with `OctopusEnergyClient`).

Do not pass different keys to the handler and the client; when both are configured, the client key is applied first and the handler does not replace an existing `Authorization` header.

### Handler carries auth

Use this when other code in your host also resolves the named `HttpClient` and needs the same authentication:

```csharp
using Microsoft.Extensions.DependencyInjection;
using OctopusEnergy.Client;

const string apiKey = Environment.GetEnvironmentVariable("OCTOPUS_ENERGY_API_KEY")
    ?? throw new InvalidOperationException("Set OCTOPUS_ENERGY_API_KEY.");

services.AddHttpClient("OctopusEnergy", client =>
{
    client.BaseAddress = new Uri(OctopusEnergyClient.DefaultBaseUrl);
})
.AddHttpMessageHandler(() => new OctopusEnergyClientHandler(apiKey));

// Resolve from the factory when constructing the SDK client:
IHttpClientFactory httpClientFactory = services.BuildServiceProvider()
    .GetRequiredService<IHttpClientFactory>();
using HttpClient httpClient = httpClientFactory.CreateClient("OctopusEnergy");
using OctopusEnergyClient client = new(httpClient);
```

### Client carries auth

Use this when only `OctopusEnergyClient` consumes the named client:

```csharp
services.AddHttpClient("OctopusEnergy", client =>
{
    client.BaseAddress = new Uri(OctopusEnergyClient.DefaultBaseUrl);
});

IHttpClientFactory httpClientFactory = services.BuildServiceProvider()
    .GetRequiredService<IHttpClientFactory>();
using HttpClient httpClient = httpClientFactory.CreateClient("OctopusEnergy");
using OctopusEnergyClient client = new(apiKey, httpClient);
```

For public catalogue calls only, omit the API key on both the handler and the client:

```csharp
services.AddHttpClient("OctopusEnergy", client =>
{
    client.BaseAddress = new Uri(OctopusEnergyClient.DefaultBaseUrl);
})
.AddHttpMessageHandler(() => new OctopusEnergyClientHandler());
```

Set `BaseAddress` on the named client at registration time. The SDK normalises trailing slashes when resolving relative paths but does not rewrite `BaseAddress` on a factory-created instance.

Do not call `new HttpClient()` per request; resolve clients from the factory (or use `new OctopusEnergyClient()` for short-lived console tools).

## Supplying your own `HttpClient`

When you pass an `HttpClient` directly, the SDK does **not** modify `DefaultRequestHeaders` or `BaseAddress` on the instance you supply. Authentication, `Accept`, and `User-Agent` are applied per request. If `BaseAddress` is null, the default UK API URL is used internally for relative paths.

This is a behavioural change from earlier releases, which mutated `DefaultRequestHeaders` and `BaseAddress` on supplied clients. That mutation caused credential and header leakage when sharing a factory-created `HttpClient` across components.

```csharp
using var httpClient = new HttpClient { BaseAddress = new Uri("https://api.example.test/v1/") };
using var client = new OctopusEnergyClient(apiKey, httpClient);
```

When `DefaultRequestHeaders` already includes `Accept: application/json`, the SDK does not add a second JSON `Accept` value on each request.

See also [pagination](pagination.md#supplying-your-own-httpclient).

## Protect the key in your application

- Do not commit API keys to source control or test fixtures.
- Store keys in environment variables, user secrets, or your host's secret manager.
- Custom `HttpClient` handlers, logging middleware, or diagnostic tools may capture the `Authorization` header. Review anything that logs outbound HTTP requests before enabling it in production.

GraphQL JWT exchange is a v2 implementation detail. v1 REST uses Basic only.

See [coding notes](https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/planning/coding-notes.md) (section 3, REST).
