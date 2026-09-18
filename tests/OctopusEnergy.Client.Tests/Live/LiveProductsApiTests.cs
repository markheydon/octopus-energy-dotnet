using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Live;

/// <summary>
/// Optional smoke tests against the live UK API. Skipped unless
/// <see cref="LiveTestGate.EnableEnvironmentVariable"/> is set.
/// </summary>
public sealed class LiveProductsApiTests
{
    [Fact]
    public async Task ListAsync_WhenEnabled_ReturnsAtLeastOneProduct()
    {
        LiveTestGate.SkipUnlessEnabled();

        using OctopusEnergyClient client = new();
        Product? first = null;

        await foreach (Product product in client.Products.ListAsync(cancellationToken: CancellationToken.None))
        {
            first = product;
            break;
        }

        Assert.NotNull(first);
        Assert.False(string.IsNullOrWhiteSpace(first.Code));
    }
}
