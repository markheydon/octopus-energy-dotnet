#:project ../../src/OctopusEnergy.Client/OctopusEnergy.Client.csproj
#:property PublishAot=false

using OctopusEnergy.Client;

using var client = new OctopusEnergyClient();

int count = 0;
await foreach (var product in client.Products.ListAsync())
{
    count++;
    if (count == 1)
    {
        Console.WriteLine(product.DisplayName);
    }

    if (count >= 5)
    {
        break;
    }
}

if (count == 0)
{
    throw new InvalidOperationException("Expected at least one product from the public catalogue.");
}
