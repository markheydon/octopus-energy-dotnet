using System.Text.Json;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.JsonSourceGeneration.Tests;

public sealed class ReflectionDisabledDeserializationTests
{
    [Fact]
    public void Deserialize_ProductsListPage_Succeeds()
    {
        PaginatedResponse<Product> page = Deserialize<PaginatedResponse<Product>>("products-list-page-1.json");

        Assert.True(page.Count > 0);
        Assert.NotEmpty(page.Results);
        Assert.False(string.IsNullOrWhiteSpace(page.Results[0].Code));
    }

    [Fact]
    public void Deserialize_ProductAgileDetail_Succeeds()
    {
        ProductDetail detail = Deserialize<ProductDetail>("products-agile-detail.json");

        Assert.False(string.IsNullOrWhiteSpace(detail.Code));
        Assert.NotEmpty(detail.SingleRegisterElectricityTariffs);
    }

    [Fact]
    public void Deserialize_AccountDetail_Succeeds()
    {
        Account account = Deserialize<Account>("accounts-detail.json");

        Assert.False(string.IsNullOrWhiteSpace(account.Number));
        Assert.NotEmpty(account.Properties);
    }

    [Fact]
    public void Deserialize_ConsumptionPage_Succeeds()
    {
        PaginatedResponse<ConsumptionInterval> page =
            Deserialize<PaginatedResponse<ConsumptionInterval>>("consumption-bst-spring-forward.json");

        Assert.Equal(4, page.Results.Count);
        Assert.True(page.Results[0].Consumption > 0);
    }

    [Fact]
    public void Deserialize_TariffRatesPage_Succeeds()
    {
        PaginatedResponse<TariffCharge> page =
            Deserialize<PaginatedResponse<TariffCharge>>("tariff-rates-agile-half-hours.json");

        Assert.NotEmpty(page.Results);
    }

    [Fact]
    public void Deserialize_IndustryGridSupplyPoints_Succeeds()
    {
        PaginatedResponse<GridSupplyPointLookup> page =
            Deserialize<PaginatedResponse<GridSupplyPointLookup>>("industry-grid-supply-points-london.json");

        Assert.NotEmpty(page.Results);
    }

    [Fact]
    public void Deserialize_ElectricityMeterPointLookup_Succeeds()
    {
        ElectricityMeterPointLookup lookup =
            Deserialize<ElectricityMeterPointLookup>("electricity-meter-point-london.json");

        Assert.True(lookup.ProfileClass > 0);
    }

    [Fact]
    public void Deserialize_ApiError_Succeeds()
    {
        ApiErrorResponse error = Deserialize<ApiErrorResponse>("api-error-not-found.json");

        Assert.False(string.IsNullOrWhiteSpace(error.Detail));
    }

    private static T Deserialize<T>(string fixtureName)
    {
        string fullPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", fixtureName);
        string json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<T>(json, OctopusJsonSerializerOptions.Default)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null.");
    }
}
