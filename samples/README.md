# Samples

Opt-in console apps that call the live Octopus Energy UK API. They are for local smoke checks by humans and agents with a dashboard API key. CI does not run them.

## Products console

Lists the first five products from the catalogue and fetches detail for the first result.

### Prerequisites

- .NET 10.0 SDK
- An Octopus dashboard API key ([create one](https://octopus.energy/dashboard/new/accounts/personal-details/api-access))

### Run

```bash
export OCTOPUS_ENERGY_API_KEY="your-key-here"
dotnet run --project samples/ProductsConsole
```

PowerShell:

```powershell
$env:OCTOPUS_ENERGY_API_KEY = "your-key-here"
dotnet run --project samples/ProductsConsole
```

The sample uses the authenticated client constructor so the HTTP Basic path is exercised even though the products catalogue is public. It hits `https://api.octopus.energy/v1/` and does not log your key.

### More samples

Additional samples may be added later (for example account or consumption) as the SDK surface grows. Each sample should reflect implemented SDK behaviour only.
