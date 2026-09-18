# Samples

Opt-in console apps that call the live Octopus Energy UK API. They are for local smoke checks by humans and agents. CI does not run them.

## Products console

Smoke-tests the products catalogue and optionally account detail against `https://api.octopus.energy/v1/`:

1. **Authenticated path (optional, runs first when a key is set)** — when `OCTOPUS_ENERGY_API_KEY` and `OCTOPUS_ENERGY_ACCOUNT_NUMBER` are set, `new OctopusEnergyClient(apiKey)` fetches account detail and prints a small summary (account number, property count, meter-point counts). When only the API key is set, the sample skips the account call and explains how to set the account number. This runs before the public section so an invalid key fails fast.
2. **Public path (always)** — `new OctopusEnergyClient()` lists five products and fetches detail for the first. No API key required; the catalogue is public.

### Prerequisites

- .NET 10.0 SDK
- Optional: Octopus dashboard API key ([create one](https://octopus.energy/dashboard/new/accounts/personal-details/api-access))
- Optional: account number (`A-XXXXXXXX`) for the authenticated account smoke

### Run

Without a key (public catalogue only):

```bash
dotnet run --project samples/ProductsConsole
```

With a key and account number (account detail smoke first, then public catalogue):

```bash
export OCTOPUS_ENERGY_API_KEY="your-key-here"
export OCTOPUS_ENERGY_ACCOUNT_NUMBER="A-12345678"
dotnet run --project samples/ProductsConsole
```

PowerShell:

```powershell
$env:OCTOPUS_ENERGY_API_KEY = "your-key-here"
$env:OCTOPUS_ENERGY_ACCOUNT_NUMBER = "A-12345678"
dotnet run --project samples/ProductsConsole
```

The sample never logs your key.

### More samples

Additional samples may be added later (for example consumption) as the SDK surface grows. Each sample should reflect implemented SDK behaviour only.
