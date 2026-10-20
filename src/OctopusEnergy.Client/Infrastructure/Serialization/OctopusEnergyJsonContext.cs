using System.Text.Json.Serialization;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(ApiErrorResponse))]
[JsonSerializable(typeof(PaginatedResponse<Product>))]
[JsonSerializable(typeof(PaginatedResponse<TariffCharge>))]
[JsonSerializable(typeof(PaginatedResponse<ConsumptionInterval>))]
[JsonSerializable(typeof(PaginatedResponse<GridSupplyPointLookup>))]
[JsonSerializable(typeof(Product))]
[JsonSerializable(typeof(ProductDetail))]
[JsonSerializable(typeof(ProductLink))]
[JsonSerializable(typeof(ProductTariff))]
[JsonSerializable(typeof(ProductPaymentMethodTariffs))]
[JsonSerializable(typeof(ProductSampleQuotesByPaymentMethod))]
[JsonSerializable(typeof(ProductSampleRateQuote))]
[JsonSerializable(typeof(ProductSampleAnnualCost))]
[JsonSerializable(typeof(ProductSampleConsumption))]
[JsonSerializable(typeof(ProductSampleConsumptionProfile))]
[JsonSerializable(typeof(TariffCharge))]
[JsonSerializable(typeof(Account))]
[JsonSerializable(typeof(AccountProperty))]
[JsonSerializable(typeof(ElectricityMeterPoint))]
[JsonSerializable(typeof(GasMeterPoint))]
[JsonSerializable(typeof(ElectricityMeter))]
[JsonSerializable(typeof(GasMeter))]
[JsonSerializable(typeof(MeterRegister))]
[JsonSerializable(typeof(TariffAgreement))]
[JsonSerializable(typeof(ConsumptionInterval))]
[JsonSerializable(typeof(GridSupplyPointLookup))]
[JsonSerializable(typeof(ElectricityMeterPointLookup))]
[JsonSerializable(typeof(GridSupplyPoint))]
internal partial class OctopusEnergyJsonContext : JsonSerializerContext;
