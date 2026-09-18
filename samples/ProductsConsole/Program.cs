using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;

const string ApiKeyEnvironmentVariable = "OCTOPUS_ENERGY_API_KEY";
const int MaxProductsToList = 5;

using CancellationTokenSource cancellation = new();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    cancellation.Cancel();
};

string? apiKey = Environment.GetEnvironmentVariable(ApiKeyEnvironmentVariable);
CancellationToken cancellationToken = cancellation.Token;

try
{
    Console.WriteLine("=== Public catalogue (no API key) ===");
    Console.WriteLine();
    Console.WriteLine(
        "Products are public. This section uses {0}().",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    using (OctopusEnergyClient publicClient = new())
    {
        string productCode = await RunProductsSmokeAsync(
            publicClient,
            MaxProductsToList,
            cancellationToken);

        ProductDetail detail = await publicClient.Products.GetAsync(productCode, cancellationToken: cancellationToken);
        WriteProductDetail(detail);
    }

    Console.WriteLine();

    if (string.IsNullOrWhiteSpace(apiKey))
    {
        Console.WriteLine("=== Authenticated client (skipped) ===");
        Console.WriteLine();
        WriteOptionalApiKeyHelp(ApiKeyEnvironmentVariable);
        return 0;
    }

    Console.WriteLine("=== Authenticated client (API key) ===");
    Console.WriteLine();
    Console.WriteLine(
        "Account and consumption services are not in the SDK yet. This section uses {0}(apiKey) against the same public catalogue to smoke-test HTTP Basic auth.",
        nameof(OctopusEnergyClient));
    Console.WriteLine();

    using (OctopusEnergyClient authenticatedClient = new(apiKey))
    {
        string productCode = await RunProductsSmokeAsync(
            authenticatedClient,
            1,
            cancellationToken);

        Console.WriteLine("Authenticated list succeeded (first product: {0}).", productCode);
    }

    return 0;
}
catch (OctopusEnergyException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
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

static void WriteOptionalApiKeyHelp(string environmentVariable)
{
    Console.WriteLine(
        "Set {0} to also exercise the authenticated client constructor (HTTP Basic auth).",
        environmentVariable);
    Console.WriteLine();
    Console.WriteLine("Create a key:");
    Console.WriteLine("  https://octopus.energy/dashboard/new/accounts/personal-details/api-access");
    Console.WriteLine();
    Console.WriteLine("Bash:");
    Console.WriteLine("  export {0}=\"your-key-here\"", environmentVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
    Console.WriteLine();
    Console.WriteLine("PowerShell:");
    Console.WriteLine("  $env:{0} = \"your-key-here\"", environmentVariable);
    Console.WriteLine("  dotnet run --project samples/ProductsConsole");
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
