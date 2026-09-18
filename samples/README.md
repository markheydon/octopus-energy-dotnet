# Samples

Opt-in console apps that call the live Octopus Energy UK API. They are for local smoke checks by humans and agents. CI does not run them.

## Products console

Smoke-tests the products catalogue against `https://api.octopus.energy/v1/`:

1. **Public path (always)** — `new OctopusEnergyClient()` lists five products and fetches detail for the first. No API key required; the catalogue is public.
2. **Authenticated path (optional)** — when `OCTOPUS_ENERGY_API_KEY` is set, `new OctopusEnergyClient(apiKey)` lists one product to exercise HTTP Basic auth. Account and consumption are not in the SDK yet, so there is no key-required resource call to make today.

### Prerequisites

- .NET 10.0 SDK
- Optional: Octopus dashboard API key ([create one](https://octopus.energy/dashboard/new/accounts/personal-details/api-access))

### Run

Without a key (public catalogue only):

```bash
dotnet run --project samples/ProductsConsole
```

With a key (public catalogue plus authenticated client smoke):

```bash
export OCTOPUS_ENERGY_API_KEY="your-key-here"
dotnet run --project samples/ProductsConsole
```

PowerShell:

```powershell
$env:OCTOPUS_ENERGY_API_KEY = "your-key-here"
dotnet run --project samples/ProductsConsole
```

The sample never logs your key.

### More samples

Additional samples may be added later (for example account or consumption) as the SDK surface grows. Each sample should reflect implemented SDK behaviour only.
