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
if (string.IsNullOrWhiteSpace(apiKey))
{
    WriteMissingApiKeyHelp(ApiKeyEnvironmentVariable);
    return 1;
}

try
{
    using OctopusEnergyClient client = new(apiKey);
    CancellationToken cancellationToken = cancellation.Token;

    Console.WriteLine("Listing products (first {0}):", MaxProductsToList);
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

        if (products.Count >= MaxProductsToList)
        {
            break;
        }
    }

    if (products.Count == 0)
    {
        Console.Error.WriteLine("No products returned from the catalogue.");
        return 1;
    }

    string productCode = products[0].Code;
    Console.WriteLine();
    Console.WriteLine("Product detail for {0}:", productCode);
    Console.WriteLine();

    ProductDetail detail = await client.Products.GetAsync(productCode, cancellationToken: cancellationToken);
    WriteProductDetail(detail);

    return 0;
}
catch (OctopusEnergyException ex)
{
    Console.Error.WriteLine(ex.Message);
    return 1;
}

static void WriteMissingApiKeyHelp(string environmentVariable)
{
    Console.Error.WriteLine("Set an Octopus dashboard API key in the {0} environment variable.", environmentVariable);
    Console.Error.WriteLine();
    Console.Error.WriteLine("Create a key:");
    Console.Error.WriteLine("  https://octopus.energy/dashboard/new/accounts/personal-details/api-access");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Bash:");
    Console.Error.WriteLine("  export {0}=\"your-key-here\"", environmentVariable);
    Console.Error.WriteLine("  dotnet run --project samples/ProductsConsole");
    Console.Error.WriteLine();
    Console.Error.WriteLine("PowerShell:");
    Console.Error.WriteLine("  $env:{0} = \"your-key-here\"", environmentVariable);
    Console.Error.WriteLine("  dotnet run --project samples/ProductsConsole");
}

static void WriteProductDetail(ProductDetail detail)
{
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
