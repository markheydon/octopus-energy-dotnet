using System.Net.Http;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Products;

const string ApiKeyEnvironmentVariable = "OCTOPUS_ENERGY_API_KEY";
const string AccountNumberEnvironmentVariable = "OCTOPUS_ENERGY_ACCOUNT_NUMBER";
const int MaxProductsToList = 5;

using CancellationTokenSource cancellation = new();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
string? accountNumber = Environment.GetEnvironmentVariable(AccountNumberEnvironmentVariable);
CancellationToken cancellationToken = cancellation.Token;

try
{
    bool hasApiKey = !string.IsNullOrWhiteSpace(apiKey);
    bool hasAccountNumber = !string.IsNullOrWhiteSpace(accountNumber);

    if (hasApiKey)
    {
        int authenticatedExitCode = await TryRunSectionAsync(
            "authenticated client",
            () => RunAuthenticatedSmokeAsync(apiKey!, accountNumber, cancellationToken));

        if (authenticatedExitCode != 0)
        {
            return authenticatedExitCode;
        }

        Console.WriteLine();
    }

    int publicExitCode = await TryRunSectionAsync(
        "public catalogue",
        () => RunPublicSmokeAsync(cancellationToken));

    if (publicExitCode != 0)
    {
        return publicExitCode;
    }

    Console.WriteLine();

    if (!hasApiKey)
    {
        Console.WriteLine("=== Authenticated client (skipped) ===");
        Console.WriteLine();
        WriteOptionalApiKeyHelp(ApiKeyEnvironmentVariable, AccountNumberEnvironmentVariable);
        return 0;
    }

    if (hasApiKey && hasAccountNumber)
    {
        Console.WriteLine("Smoke check succeeded (account detail and public catalogue).");
    }
    else
    {
        Console.WriteLine("Smoke check succeeded (public catalogue).");
    }

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

static async Task RunPublicSmokeAsync(CancellationToken cancellationToken)
{
    Console.WriteLine("=== Public catalogue (no API key) ===");
    Console.WriteLine();
    Console.WriteLine(
        "Products are public. This section uses {0}().",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    using OctopusEnergyClient publicClient = new();
    string productCode = await RunProductsSmokeAsync(
        publicClient,
        MaxProductsToList,
        cancellationToken);

    ProductDetail detail = await publicClient.Products.GetAsync(productCode, cancellationToken: cancellationToken);
    WriteProductDetail(detail);
}

static async Task RunAuthenticatedSmokeAsync(
    string apiKey,
    string? accountNumber,
    CancellationToken cancellationToken)
{
    Console.WriteLine("=== Authenticated client (API key) ===");
    Console.WriteLine();

    using OctopusEnergyClient authenticatedClient = new(apiKey);

    if (string.IsNullOrWhiteSpace(accountNumber))
    {
        Console.WriteLine(
            "Set {0} to fetch account detail. Skipping account call.",
            AccountNumberEnvironmentVariable);
        Console.WriteLine();
        WriteOptionalAccountNumberHelp(AccountNumberEnvironmentVariable);
        return;
    }

    Console.WriteLine(
        "Fetching account detail with {0}(apiKey).",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    Account account = await authenticatedClient.Accounts.GetAsync(accountNumber, cancellationToken);
    WriteAccountSummary(account);
}

static async Task<string> RunProductsSmokeAsync(
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

static void WriteOptionalApiKeyHelp(string apiKeyVariable, string accountNumberVariable)
{
    Console.WriteLine(
        "Set {0} to exercise the authenticated client constructor (HTTP Basic auth).",
        apiKeyVariable);
    Console.WriteLine(
        "Set {0} as well to fetch account detail.",
        accountNumberVariable);
    Console.WriteLine();
    Console.WriteLine("Create a key:");
    Console.WriteLine("  https://octopus.energy/dashboard/new/accounts/personal-details/api-access");
    Console.WriteLine();
    Console.WriteLine("Bash:");
    Console.WriteLine("  export {0}=\"your-key-here\"", apiKeyVariable);
    Console.WriteLine("  export {0}=\"A-12345678\"", accountNumberVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
    Console.WriteLine();
    Console.WriteLine("PowerShell:");
    Console.WriteLine("  $env:{0} = \"your-key-here\"", apiKeyVariable);
    Console.WriteLine("  $env:{0} = \"A-12345678\"", accountNumberVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
}

static void WriteOptionalAccountNumberHelp(string accountNumberVariable)
{
    Console.WriteLine(
        "Your account number is on your bill or in the Octopus dashboard (format A-XXXXXXXX).");
    Console.WriteLine();
    Console.WriteLine("Bash:");
    Console.WriteLine("  export {0}=\"A-12345678\"", accountNumberVariable);
    Console.WriteLine();
    Console.WriteLine("PowerShell:");
    Console.WriteLine("  $env:{0} = \"A-12345678\"", accountNumberVariable);
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
