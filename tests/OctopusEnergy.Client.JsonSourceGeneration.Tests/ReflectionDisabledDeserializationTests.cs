using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.GraphQL;
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
        Assert.Equal(ProductEnergyDirection.Import, page.Results[0].Direction!.Value.Value);
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
        Assert.Equal(
            MeterRegisterRate.Standard,
            account.Properties[0].ElectricityMeterPoints[0].Meters[0].Registers[0].Rate.Value);
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
        Assert.Null(page.Results[0].PaymentMethod);
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

    [Fact]
    public void Deserialize_OctoplusAccountInfoEnvelope_Succeeds()
    {
        GraphQlResponseEnvelope<OctoplusAccountInfoQueryData> envelope =
            DeserializeEnvelope<OctoplusAccountInfoQueryData>("graphql-octoplus-account-info.json");

        Assert.NotNull(envelope.Data?.OctoplusAccountInfo);
    }

    [Fact]
    public void Deserialize_SmartMeterTelemetryEnvelope_Succeeds()
    {
        GraphQlResponseEnvelope<SmartMeterTelemetryQueryData> envelope =
            DeserializeEnvelope<SmartMeterTelemetryQueryData>("graphql-smart-meter-telemetry.json");

        Assert.NotNull(envelope.Data?.SmartMeterTelemetry);
    }

    [Fact]
    public void Deserialize_InitiateProductSwitchEnvelope_Succeeds()
    {
        GraphQlResponseEnvelope<InitiateProductSwitchMutationData> envelope =
            DeserializeEnvelope<InitiateProductSwitchMutationData>("graphql-initiate-product-switch-success.json");

        Assert.NotNull(envelope.Data?.InitiateProductSwitch);
    }

    [Fact]
    public void Serialize_GraphQlRequestEnvelope_OctoplusVariables_ProducesJson()
    {
        GraphQlRequestEnvelope<OctoplusAccountInfoQueryVariables> body = new()
        {
            Query = "query { }",
            Variables = new OctoplusAccountInfoQueryVariables { AccountNumber = "A-12345678" },
        };

        JsonTypeInfo typeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlRequestEnvelope<OctoplusAccountInfoQueryVariables>))
            ?? throw new InvalidOperationException("Missing type info.");

        string json = JsonSerializer.Serialize(body, typeInfo);
        Assert.Contains("A-12345678", json, StringComparison.Ordinal);
        Assert.Contains("query", json, StringComparison.Ordinal);
    }

    private static T Deserialize<T>(string fixtureName)
    {
        string fullPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", fixtureName);
        string json = File.ReadAllText(fullPath);
        return (T)(JsonSerializer.Deserialize(json, OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(T))!)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null."));
    }

    private static GraphQlResponseEnvelope<TData> DeserializeEnvelope<TData>(string fixtureName)
    {
        string fullPath = Path.Combine(AppContext.BaseDirectory, "Fixtures", fixtureName);
        string json = File.ReadAllText(fullPath);
        return (GraphQlResponseEnvelope<TData>)(JsonSerializer.Deserialize(
                   json,
                   OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlResponseEnvelope<TData>))!)
            ?? throw new InvalidOperationException($"Fixture {fixtureName} deserialised to null."));
    }
}
