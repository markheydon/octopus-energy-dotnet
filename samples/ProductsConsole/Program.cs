using System.Net.Http;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Models.Products;

const string ApiKeyEnvironmentVariable = "OCTOPUS_ENERGY_API_KEY";
const string AccountNumberEnvironmentVariable = "OCTOPUS_ENERGY_ACCOUNT_NUMBER";
const string PostcodeEnvironmentVariable = "OCTOPUS_ENERGY_POSTCODE";
const string MpanEnvironmentVariable = "OCTOPUS_ENERGY_MPAN";
const string ElectricityMeterSerialEnvironmentVariable = "OCTOPUS_ENERGY_ELECTRICITY_METER_SERIAL";
const string MprnEnvironmentVariable = "OCTOPUS_ENERGY_MPRN";
const string GasMeterSerialEnvironmentVariable = "OCTOPUS_ENERGY_GAS_METER_SERIAL";
const string DefaultPostcode = "W1 1AA";
const int MaxProductsToList = 5;
const int TariffRatesPageSize = 1;
const int ConsumptionPageSize = 1;

using CancellationTokenSource cancellation = new();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
string? accountNumber = Environment.GetEnvironmentVariable(AccountNumberEnvironmentVariable);
string postcode = Environment.GetEnvironmentVariable(PostcodeEnvironmentVariable) ?? DefaultPostcode;
string? mpanOverride = Environment.GetEnvironmentVariable(MpanEnvironmentVariable);
string? electricityMeterSerialOverride = Environment.GetEnvironmentVariable(ElectricityMeterSerialEnvironmentVariable);
string? mprnOverride = Environment.GetEnvironmentVariable(MprnEnvironmentVariable);
string? gasMeterSerialOverride = Environment.GetEnvironmentVariable(GasMeterSerialEnvironmentVariable);
CancellationToken cancellationToken = cancellation.Token;

try
{
    bool hasApiKey = !string.IsNullOrWhiteSpace(apiKey);
    bool hasAccountNumber = !string.IsNullOrWhiteSpace(accountNumber);
    Account? account = null;
    ProductDetail? productDetail = null;

    if (hasApiKey)
    {
        int authenticatedExitCode = await TryRunSectionAsync(
            "authenticated client",
            async () =>
            {
                account = await RunAuthenticatedSmokeAsync(
                    apiKey!,
                    accountNumber,
                    mpanOverride,
                    electricityMeterSerialOverride,
                    mprnOverride,
                    gasMeterSerialOverride,
                    cancellationToken);
            });

        if (authenticatedExitCode != 0)
        {
            return authenticatedExitCode;
        }

        Console.WriteLine();
    }

    int productsExitCode = await TryRunSectionAsync(
        "public catalogue",
        async () =>
        {
            productDetail = await RunProductsSmokeAsync(cancellationToken);
        });

    if (productsExitCode != 0)
    {
        return productsExitCode;
    }

    Console.WriteLine();

    int tariffRatesExitCode = await TryRunSectionAsync(
        "tariff rates",
        () => RunTariffRatesSmokeAsync(productDetail!, cancellationToken));

    if (tariffRatesExitCode != 0)
    {
        return tariffRatesExitCode;
    }

    Console.WriteLine();

    int industryExitCode = await TryRunSectionAsync(
        "industry lookups",
        () => RunIndustrySmokeAsync(postcode, mpanOverride, cancellationToken));

    if (industryExitCode != 0)
    {
        return industryExitCode;
    }

    Console.WriteLine();

    if (!hasApiKey)
    {
        Console.WriteLine("=== Authenticated endpoints (skipped) ===");
        Console.WriteLine();
        WriteOptionalApiKeyHelp();
        return 0;
    }

    Console.WriteLine("Smoke check succeeded.");
    return 0;
}
catch (OperationCanceledException)
{
    return 130;
}

static async Task<int> TryRunSectionAsync(string section, Func<Task> action)
{
    try
    {
        await action();
        return 0;
    }
    catch (OperationCanceledException)
    {
        throw;
    }
    catch (HttpRequestException ex)
    {
        WriteSmokeFailure(section, ex.Message);
        return 1;
    }
    catch (OctopusEnergyException ex)
    {
        WriteSmokeFailure(section, ex.Message);
        return 1;
    }
}

static async Task<Account?> RunAuthenticatedSmokeAsync(
    string apiKey,
    string? accountNumber,
    string? mpanOverride,
    string? electricityMeterSerialOverride,
    string? mprnOverride,
    string? gasMeterSerialOverride,
    CancellationToken cancellationToken)
{
    Console.WriteLine("=== Authenticated client (API key) ===");
    Console.WriteLine();

    using OctopusEnergyClient authenticatedClient = new(apiKey);

    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine(
            "Set {0} to fetch account detail and consumption. Listing one product to smoke-test HTTP Basic auth.",
            AccountNumberEnvironmentVariable);
        Console.WriteLine();
        WriteOptionalAccountNumberHelp();
        Console.WriteLine();

        string productCode = await RunProductsListAsync(
            authenticatedClient,
            1,
            cancellationToken);

        Console.WriteLine("Authenticated list succeeded (first product: {0}).", productCode);

        if (TryResolveElectricityConsumptionTarget(
                null,
                mpanOverride,
                electricityMeterSerialOverride,
                out string? mpan,
                out string? electricitySerial))
        {
            await RunElectricityConsumptionSmokeAsync(
                authenticatedClient,
                mpan!,
                electricitySerial!,
                cancellationToken);
        }
        else
        {
            WriteConsumptionSkipped(
                "electricity",
                $"Set {AccountNumberEnvironmentVariable} or both {MpanEnvironmentVariable} and {ElectricityMeterSerialEnvironmentVariable}.");
        }

        if (TryResolveGasConsumptionTarget(
                null,
                mprnOverride,
                gasMeterSerialOverride,
                out string? mprn,
                out string? gasSerial))
        {
            await RunGasConsumptionSmokeAsync(
                authenticatedClient,
                mprn!,
                gasSerial!,
                cancellationToken);
        }
        else
        {
            WriteConsumptionSkipped(
                "gas",
                $"Set {AccountNumberEnvironmentVariable} or both {MprnEnvironmentVariable} and {GasMeterSerialEnvironmentVariable}.");
        }

        return null;
    }

    Console.WriteLine(
        "Fetching account detail with {0}(apiKey).",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    Account account = await authenticatedClient.Accounts.GetAsync(accountNumber, cancellationToken);
    WriteAccountSummary(account);

    if (TryResolveElectricityConsumptionTarget(
            account,
            mpanOverride,
            electricityMeterSerialOverride,
            out string? accountMpan,
            out string? accountElectricitySerial))
    {
        await RunElectricityConsumptionSmokeAsync(
            authenticatedClient,
            accountMpan!,
            accountElectricitySerial!,
            cancellationToken);
    }
    else
    {
        WriteConsumptionSkipped(
            "electricity",
            "No import electricity meter with a serial number was returned on the account.");
    }

    if (TryResolveGasConsumptionTarget(
            account,
            mprnOverride,
            gasMeterSerialOverride,
            out string? accountMprn,
            out string? accountGasSerial))
    {
        await RunGasConsumptionSmokeAsync(
            authenticatedClient,
            accountMprn!,
            accountGasSerial!,
            cancellationToken);
    }
    else
    {
        WriteConsumptionSkipped("gas", "No gas meter with a serial number was returned on the account.");
    }

    return account;
}

static async Task<ProductDetail> RunProductsSmokeAsync(CancellationToken cancellationToken)
{
    Console.WriteLine("=== Public catalogue (no API key) ===");
    Console.WriteLine();
    Console.WriteLine(
        "Products are public. This section uses {0}().",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    using OctopusEnergyClient publicClient = new();
    string productCode = await RunProductsListAsync(
        publicClient,
        MaxProductsToList,
        cancellationToken);

    ProductDetail detail = await publicClient.Products.GetAsync(productCode, cancellationToken: cancellationToken);
    WriteProductDetail(detail);
    return detail;
}

static async Task RunTariffRatesSmokeAsync(ProductDetail detail, CancellationToken cancellationToken)
{
    Console.WriteLine("=== Tariff rates (no API key) ===");
    Console.WriteLine();

    string? tariffCodeValue = TryGetExampleTariffCode(detail);
    if (tariffCodeValue is null)
    {
        Console.WriteLine("Skipped: no single-register electricity tariff was returned on the product detail.");
        return;
    }

    TariffCode tariffCode = TariffCode.Parse(tariffCodeValue);
    using OctopusEnergyClient client = new();

    TariffChargeListRequest request = new() { PageSize = TariffRatesPageSize };

    TariffCharge? standingCharge = await ReadFirstAsync(
        client.TariffRates.ListStandingChargesAsync(tariffCode, request, cancellationToken));

    if (standingCharge is null)
    {
        Console.WriteLine("Standing charges: none returned for {0}.", tariffCodeValue);
    }
    else
    {
        Console.WriteLine(
            "Standing charge ({0}): {1} p/day inc VAT from {2:u}{3}",
            tariffCodeValue,
            standingCharge.ValueIncVat,
            standingCharge.ValidFrom,
            FormatValidToSuffix(standingCharge.ValidTo));
    }

    TariffCharge? unitRate = await ReadFirstAsync(
        client.TariffRates.ListStandardUnitRatesAsync(tariffCode, request, cancellationToken));

    if (unitRate is null)
    {
        Console.WriteLine("Standard unit rates: none returned for {0}.", tariffCodeValue);
    }
    else
    {
        Console.WriteLine(
            "Standard unit rate ({0}): {1} p/kWh inc VAT from {2:u}{3}",
            tariffCodeValue,
            unitRate.ValueIncVat,
            unitRate.ValidFrom,
            FormatValidToSuffix(unitRate.ValidTo));
    }
}

static async Task RunIndustrySmokeAsync(
    string postcode,
    string? mpanOverride,
    CancellationToken cancellationToken)
{
    Console.WriteLine("=== Industry lookups (no API key) ===");
    Console.WriteLine();

    using OctopusEnergyClient client = new();

    Console.WriteLine("Grid supply points for postcode {0}:", postcode);
    GridSupplyPointLookup gspLookup = await ReadFirstAsync(
        client.Industry.ListGridSupplyPointsByPostcodeAsync(postcode, cancellationToken))
        ?? throw new OctopusEnergyException($"No grid supply points returned for postcode {postcode}.");

    Console.WriteLine(
        "  GSP {0}{1}",
        gspLookup.GridSupplyPoint,
        string.IsNullOrWhiteSpace(gspLookup.Mpan) ? string.Empty : $" (example MPAN {gspLookup.Mpan})");

    string? mpan = !string.IsNullOrWhiteSpace(mpanOverride)
        ? mpanOverride
        : string.IsNullOrWhiteSpace(gspLookup.Mpan) ? null : gspLookup.Mpan;

    if (mpan is null)
    {
        Console.WriteLine();
        Console.WriteLine(
            "Electricity meter point lookup (skipped): set {0} to exercise {1}.",
            MpanEnvironmentVariable,
            nameof(client.Industry.GetElectricityMeterPointAsync));
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Electricity meter point for MPAN ending …{0}:", LastFourDigits(mpan));

    ElectricityMeterPointLookup meterPoint = await client.Industry.GetElectricityMeterPointAsync(
        mpan,
        cancellationToken);

    Console.WriteLine(
        "  GSP {0}, profile class {1}",
        meterPoint.GridSupplyPoint,
        meterPoint.ProfileClass);
}

static async Task RunElectricityConsumptionSmokeAsync(
    OctopusEnergyClient client,
    string mpan,
    string meterSerialNumber,
    CancellationToken cancellationToken)
{
    Console.WriteLine();
    Console.WriteLine(
        "Electricity consumption (MPAN ending …{0}, meter {1}):",
        LastFourDigits(mpan),
        meterSerialNumber);

    ConsumptionListRequest request = new()
    {
        PeriodFrom = DateTimeOffset.UtcNow.AddDays(-7),
        PeriodTo = DateTimeOffset.UtcNow,
        PageSize = ConsumptionPageSize,
        OrderBy = ConsumptionOrderBy.PeriodDescending,
    };

    ConsumptionInterval? interval = await ReadFirstAsync(
        client.Consumption.ListElectricityAsync(mpan, meterSerialNumber, request, cancellationToken));

    if (interval is null)
    {
        Console.WriteLine("  No intervals returned (non-smart meters may be empty).");
        return;
    }

    Console.WriteLine(
        "  Latest interval: {0} kWh from {1:u} to {2:u}",
        interval.Consumption,
        interval.IntervalStart,
        interval.IntervalEnd);
}

static async Task RunGasConsumptionSmokeAsync(
    OctopusEnergyClient client,
    string mprn,
    string meterSerialNumber,
    CancellationToken cancellationToken)
{
    Console.WriteLine();
    Console.WriteLine(
        "Gas consumption (MPRN ending …{0}, meter {1}):",
        LastFourDigits(mprn),
        meterSerialNumber);

    ConsumptionListRequest request = new()
    {
        PeriodFrom = DateTimeOffset.UtcNow.AddDays(-7),
        PeriodTo = DateTimeOffset.UtcNow,
        PageSize = ConsumptionPageSize,
        OrderBy = ConsumptionOrderBy.PeriodDescending,
    };

    ConsumptionInterval? interval = await ReadFirstAsync(
        client.Consumption.ListGasAsync(mprn, meterSerialNumber, request, cancellationToken));

    if (interval is null)
    {
        Console.WriteLine("  No intervals returned (non-smart meters may be empty).");
        return;
    }

    string units = string.IsNullOrWhiteSpace(interval.ConsumptionUnits) ? "units" : interval.ConsumptionUnits;
    Console.WriteLine(
        "  Latest interval: {0} {1} from {2:u} to {3:u}",
        interval.Consumption,
        units,
        interval.IntervalStart,
        interval.IntervalEnd);
}

static async Task<string> RunProductsListAsync(
    OctopusEnergyClient client,
    int maxProducts,
    CancellationToken cancellationToken)
{
    Console.WriteLine("Listing products (first {0}):", maxProducts);
    Console.WriteLine();

    List<Product> products = new();
    await foreach (Product product in client.Products.ListAsync(cancellationToken: cancellationToken))
    {
        products.Add(product);
        Console.WriteLine(
            "{0}: {1} ({2})",
            product.Code,
            product.DisplayName,
            product.Brand);

        if (products.Count >= maxProducts)
        {
            break;
        }
    }

    if (products.Count == 0)
    {
        throw new OctopusEnergyException("No products returned from the catalogue.");
    }

    return products[0].Code;
}

static bool TryResolveElectricityConsumptionTarget(
    Account? account,
    string? mpanOverride,
    string? meterSerialOverride,
    out string? mpan,
    out string? meterSerialNumber)
{
    mpan = null;
    meterSerialNumber = null;

    if (!string.IsNullOrWhiteSpace(mpanOverride) && !string.IsNullOrWhiteSpace(meterSerialOverride))
    {
        mpan = mpanOverride;
        meterSerialNumber = meterSerialOverride;
        return true;
    }

    if (account is null)
    {
        return false;
    }

    foreach (AccountProperty property in account.Properties)
    {
        foreach (ElectricityMeterPoint meterPoint in property.ElectricityMeterPoints)
        {
            if (meterPoint.IsExport == true || string.IsNullOrWhiteSpace(meterPoint.Mpan))
            {
                continue;
            }

            ElectricityMeter? meter = meterPoint.Meters.FirstOrDefault(
                candidate => !string.IsNullOrWhiteSpace(candidate.SerialNumber));

            if (meter is null)
            {
                continue;
            }

            mpan = meterPoint.Mpan;
            meterSerialNumber = !string.IsNullOrWhiteSpace(meterSerialOverride)
                ? meterSerialOverride
                : meter.SerialNumber;
            return true;
        }
    }

    return false;
}

static bool TryResolveGasConsumptionTarget(
    Account? account,
    string? mprnOverride,
    string? meterSerialOverride,
    out string? mprn,
    out string? meterSerialNumber)
{
    mprn = null;
    meterSerialNumber = null;

    if (!string.IsNullOrWhiteSpace(mprnOverride) && !string.IsNullOrWhiteSpace(meterSerialOverride))
    {
        mprn = mprnOverride;
        meterSerialNumber = meterSerialOverride;
        return true;
    }

    if (account is null)
    {
        return false;
    }

    foreach (AccountProperty property in account.Properties)
    {
        foreach (GasMeterPoint meterPoint in property.GasMeterPoints)
        {
            if (string.IsNullOrWhiteSpace(meterPoint.Mprn))
            {
                continue;
            }

            GasMeter? meter = meterPoint.Meters.FirstOrDefault(
                candidate => !string.IsNullOrWhiteSpace(candidate.SerialNumber));

            if (meter is null)
            {
                continue;
            }

            mprn = meterPoint.Mprn;
            meterSerialNumber = !string.IsNullOrWhiteSpace(meterSerialOverride)
                ? meterSerialOverride
                : meter.SerialNumber;
            return true;
        }
    }

    return false;
}

static string? TryGetExampleTariffCode(ProductDetail detail)
{
    if (detail.SingleRegisterElectricityTariffs.TryGetValue(
            GridSupplyPoint.C,
            out ProductPaymentMethodTariffs? londonTariffs) &&
        londonTariffs.DirectDebitMonthly is ProductTariff londonDirectDebit)
    {
        return londonDirectDebit.Code;
    }

    foreach (ProductPaymentMethodTariffs tariffs in detail.SingleRegisterElectricityTariffs.Values)
    {
        if (tariffs.DirectDebitMonthly is ProductTariff tariff)
        {
            return tariff.Code;
        }
    }

    return null;
}

static async Task<T?> ReadFirstAsync<T>(IAsyncEnumerable<T> sequence)
{
    await foreach (T item in sequence)
    {
        return item;
    }

    return default;
}

static void WriteOptionalApiKeyHelp()
{
    Console.WriteLine(
        "Set {0} to exercise account detail and consumption.",
        ApiKeyEnvironmentVariable);
    Console.WriteLine(
        "Set {0} as well to fetch account detail and derive meter identifiers.",
        AccountNumberEnvironmentVariable);
    Console.WriteLine(
        "Optional overrides: {0}, {1}, {2}, {3}, {4}.",
        PostcodeEnvironmentVariable,
        MpanEnvironmentVariable,
        ElectricityMeterSerialEnvironmentVariable,
        MprnEnvironmentVariable,
        GasMeterSerialEnvironmentVariable);
    Console.WriteLine();
    Console.WriteLine("Create a key:");
    Console.WriteLine("  https://octopus.energy/dashboard/new/accounts/personal-details/api-access");
    Console.WriteLine();
    Console.WriteLine("Bash:");
    Console.WriteLine("  export {0}=\"your-key-here\"", ApiKeyEnvironmentVariable);
    Console.WriteLine("  export {0}=\"A-12345678\"", AccountNumberEnvironmentVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
    Console.WriteLine();
    Console.WriteLine("PowerShell:");
    Console.WriteLine("  $env:{0} = \"your-key-here\"", ApiKeyEnvironmentVariable);
    Console.WriteLine("  $env:{0} = \"A-12345678\"", AccountNumberEnvironmentVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
}

static void WriteOptionalAccountNumberHelp()
{
    Console.WriteLine(
        "Your account number is on your bill or in the Octopus dashboard (format A-XXXXXXXX).");
    Console.WriteLine();
    Console.WriteLine("Bash:");
    Console.WriteLine("  export {0}=\"A-12345678\"", AccountNumberEnvironmentVariable);
    Console.WriteLine();
    Console.WriteLine("PowerShell:");
    Console.WriteLine("  $env:{0} = \"A-12345678\"", AccountNumberEnvironmentVariable);
}

static void WriteConsumptionSkipped(string fuel, string reason)
{
    Console.WriteLine();
    Console.WriteLine("{0} consumption (skipped): {1}", Capitalize(fuel), reason);
}

static void WriteAccountSummary(Account account)
{
    int propertyCount = account.Properties.Count;
    int electricityPointCount = account.Properties.Sum(
        property => property.ElectricityMeterPoints.Count);
    int gasPointCount = account.Properties.Sum(
        property => property.GasMeterPoints.Count);

    Console.WriteLine("Account number: {0}", account.Number);
    Console.WriteLine("Properties: {0}", propertyCount);
    Console.WriteLine("Electricity meter points: {0}", electricityPointCount);
    Console.WriteLine("Gas meter points: {0}", gasPointCount);
}

static void WriteProductDetail(ProductDetail detail)
{
    Console.WriteLine();
    Console.WriteLine("Product detail for {0}:", detail.Code);
    Console.WriteLine();
    Console.WriteLine("Code: {0}", detail.Code);
    Console.WriteLine("Display name: {0}", detail.DisplayName);
    Console.WriteLine(
        "Tariffs active at: {0}",
        detail.TariffsActiveAt?.ToString("O") ?? "(not returned)");

    IReadOnlyDictionary<GridSupplyPoint, ProductPaymentMethodTariffs> tariffs =
        detail.SingleRegisterElectricityTariffs;

    if (tariffs.Count > 0)
    {
        Console.WriteLine("Single-register electricity GSP regions: {0}", tariffs.Count);
    }

    if (tariffs.TryGetValue(GridSupplyPoint.C, out ProductPaymentMethodTariffs? londonTariffs) &&
        londonTariffs.DirectDebitMonthly is ProductTariff londonDirectDebit)
    {
        Console.WriteLine("Example tariff (London, direct debit monthly): {0}", londonDirectDebit.Code);
    }
}

static void WriteSmokeFailure(string section, string message)
{
    Console.Error.WriteLine("Smoke check failed ({0}): {1}", section, message);
}

static string Capitalize(string value)
{
    if (string.IsNullOrEmpty(value))
    {
        return value;
    }

    return char.ToUpperInvariant(value[0]) + value[1..];
}

static string LastFourDigits(string value)
{
    return value.Length <= 4 ? value : value[^4..];
}

static string FormatValidToSuffix(DateTimeOffset? validTo)
{
    return validTo is null ? " (open-ended)" : $" to {validTo.Value:u}";
}
