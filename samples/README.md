# Samples

Opt-in console apps that call the live Octopus Energy UK API. They are for local smoke checks by humans and agents. CI does not run them.

## Products console

Smoke-tests every implemented public SDK surface against `https://api.octopus.energy/v1/`:

| Section | SDK surface | Auth | What it does |
|---|---|---|---|
| Authenticated client | `Accounts`, `Consumption`, `Products` | API key | Account detail when an account number is set; otherwise lists one product to prove HTTP Basic auth. Electricity and gas consumption fetch the latest interval (or report empty for non-smart meters). |
| Public catalogue | `Products` | None | Lists five products and fetches detail for the first. |
| Tariff rates | `TariffRates` | None | Reads one standing charge and one standard unit rate for an example tariff from the product detail. |
| Industry lookups | `Industry` | None | Resolves GSP for a postcode, then fetches MPAN metadata. |

When `OCTOPUS_ENERGY_API_KEY` is set, authenticated sections run first so an invalid key fails fast. Public sections always run.

### Prerequisites

- .NET 10.0 SDK
- Optional: Octopus dashboard API key ([create one](https://octopus.energy/dashboard/new/accounts/personal-details/api-access))
- Optional: account number (`A-XXXXXXXX`) for account detail and consumption meter identifiers

### Environment variables

| Variable | Required | Default | Purpose |
|---|---|---|---|
| `OCTOPUS_ENERGY_API_KEY` | No | — | Enables authenticated sections (account, consumption). |
| `OCTOPUS_ENERGY_ACCOUNT_NUMBER` | No | — | Account detail and meter identifiers for consumption. |
| `OCTOPUS_ENERGY_POSTCODE` | No | `W1 1AA` | Postcode for industry GSP lookup. |
| `OCTOPUS_ENERGY_MPAN` | No | Example MPAN from GSP lookup | Override MPAN for industry lookup; pair with `OCTOPUS_ENERGY_ELECTRICITY_METER_SERIAL` for consumption without an account number. |
| `OCTOPUS_ENERGY_ELECTRICITY_METER_SERIAL` | No | From account | Electricity meter serial for consumption. |
| `OCTOPUS_ENERGY_MPRN` | No | From account | Gas MPRN for consumption without an account number. |
| `OCTOPUS_ENERGY_GAS_METER_SERIAL` | No | From account | Gas meter serial for consumption. |

The sample never logs your key.

### Run

Without a key (public catalogue, tariff rates, and industry only):

```bash
dotnet run --project samples/ProductsConsole
```

With a key and account number (full smoke):

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

### More samples

Additional samples may be added later as the SDK surface grows. Each sample should reflect implemented SDK behaviour only.
