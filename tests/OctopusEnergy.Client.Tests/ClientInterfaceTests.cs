using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.AccountBilling;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Common;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Devices;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Models.MeterReadings;
using OctopusEnergy.Client.Models.Octoplus;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Models.ProductSwitch;
using OctopusEnergy.Client.Models.RateLimits;
using OctopusEnergy.Client.Models.SmartMeterTelemetry;
using OctopusEnergy.Client.Models.Viewer;
using OctopusEnergy.Client.Services.AccountBilling;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
using OctopusEnergy.Client.Services.Devices;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.MeterReadings;
using OctopusEnergy.Client.Services.Octoplus;
using OctopusEnergy.Client.Services.Products;
using OctopusEnergy.Client.Services.ProductSwitch;
using OctopusEnergy.Client.Services.RateLimits;
using OctopusEnergy.Client.Services.SmartMeterTelemetry;
using OctopusEnergy.Client.Services.Viewer;

namespace OctopusEnergy.Client.Tests;

public sealed class ClientInterfaceTests
{
    [Fact]
    public void OctopusEnergyClient_ImplementsIOctopusEnergyClient()
    {
        using OctopusEnergyClient client = new();

        Assert.IsAssignableFrom<IOctopusEnergyClient>(client);
        Assert.IsAssignableFrom<IAccountService>(client.Accounts);
        Assert.IsAssignableFrom<IConsumptionService>(client.Consumption);
        Assert.IsAssignableFrom<IIndustryService>(client.Industry);
        Assert.IsAssignableFrom<IProductService>(client.Products);
        Assert.IsAssignableFrom<ITariffRatesService>(client.TariffRates);
        Assert.IsAssignableFrom<IRateLimitService>(client.RateLimits);
        Assert.IsAssignableFrom<IMeterReadingService>(client.MeterReadings);
        Assert.IsAssignableFrom<IViewerService>(client.Viewer);
        Assert.IsAssignableFrom<IAccountBillingService>(client.AccountBilling);
        Assert.IsAssignableFrom<IDeviceService>(client.Devices);
        Assert.IsAssignableFrom<ISmartMeterTelemetryService>(client.SmartMeterTelemetry);
        Assert.IsAssignableFrom<IOctoplusService>(client.Octoplus);
        Assert.IsAssignableFrom<IProductSwitchService>(client.ProductSwitch);
    }

    [Fact]
    public void ResourceServices_ExposeConcreteImplementations()
    {
        using OctopusEnergyClient client = new();

        Assert.IsType<AccountService>(client.Accounts);
        Assert.IsType<ConsumptionService>(client.Consumption);
        Assert.IsType<IndustryService>(client.Industry);
        Assert.IsType<ProductService>(client.Products);
        Assert.IsType<TariffRatesService>(client.TariffRates);
        Assert.IsType<RateLimitService>(client.RateLimits);
        Assert.IsType<MeterReadingService>(client.MeterReadings);
        Assert.IsType<ViewerService>(client.Viewer);
        Assert.IsType<AccountBillingService>(client.AccountBilling);
        Assert.IsType<DeviceService>(client.Devices);
        Assert.IsType<SmartMeterTelemetryService>(client.SmartMeterTelemetry);
        Assert.IsType<OctoplusService>(client.Octoplus);
        Assert.IsType<ProductSwitchService>(client.ProductSwitch);
    }

    [Fact]
    public async Task IOctopusEnergyClient_CanBeSubstitutedInTests()
    {
        Account expectedAccount = new() { Number = "A-12345678" };
        StubOctopusEnergyClient stub = new(expectedAccount);

        Account account = await stub.Accounts.GetAsync("A-12345678", CancellationToken.None);

        Assert.Equal(expectedAccount.Number, account.Number);
        Assert.Equal("A-12345678", stub.LastRequestedAccountNumber);
    }

    private sealed class StubOctopusEnergyClient : IOctopusEnergyClient
    {
        private readonly Account _account;

        public StubOctopusEnergyClient(Account account)
        {
            _account = account;
            Accounts = new StubAccountService(account, number => LastRequestedAccountNumber = number);
            Consumption = new StubConsumptionService();
            Industry = new StubIndustryService();
            Products = new StubProductService();
            TariffRates = new StubTariffRatesService();
            RateLimits = new StubRateLimitService();
            MeterReadings = new StubMeterReadingService();
            Viewer = new StubViewerService();
            AccountBilling = new StubAccountBillingService();
            Devices = new StubDeviceService();
            SmartMeterTelemetry = new StubSmartMeterTelemetryService();
            Octoplus = new StubOctoplusService();
            ProductSwitch = new StubProductSwitchService();
        }

        public string? LastRequestedAccountNumber { get; private set; }

        public IAccountService Accounts { get; }

        public IConsumptionService Consumption { get; }

        public IIndustryService Industry { get; }

        public IProductService Products { get; }

        public ITariffRatesService TariffRates { get; }

        public IRateLimitService RateLimits { get; }

        public IMeterReadingService MeterReadings { get; }

        public IViewerService Viewer { get; }

        public IAccountBillingService AccountBilling { get; }

        public IDeviceService Devices { get; }

        public ISmartMeterTelemetryService SmartMeterTelemetry { get; }

        public IOctoplusService Octoplus { get; }

        public IProductSwitchService ProductSwitch { get; }

        public void Dispose()
        {
        }
    }

    private sealed class StubAccountService : IAccountService
    {
        private readonly Account _account;
        private readonly Action<string> _onGet;

        public StubAccountService(Account account, Action<string> onGet)
        {
            _account = account;
            _onGet = onGet;
        }

        public Task<Account> GetAsync(string accountNumber, CancellationToken cancellationToken = default)
        {
            _onGet(accountNumber);
            return Task.FromResult(_account);
        }
    }

    private sealed class StubConsumptionService : IConsumptionService
    {
        public IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
            string mpan,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyIntervals();

        public IAsyncEnumerable<ConsumptionInterval> ListElectricityAsync(
            ElectricityMeterPoint meterPoint,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyIntervals();

        public Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
            string mpan,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(EmptyPage());

        public Task<PaginatedResult<ConsumptionInterval>> ListElectricityPageAsync(
            ElectricityMeterPoint meterPoint,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(EmptyPage());

        public IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
            string mprn,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyIntervals();

        public IAsyncEnumerable<ConsumptionInterval> ListGasAsync(
            GasMeterPoint meterPoint,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyIntervals();

        public Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
            string mprn,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(EmptyPage());

        public Task<PaginatedResult<ConsumptionInterval>> ListGasPageAsync(
            GasMeterPoint meterPoint,
            string meterSerialNumber,
            ConsumptionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(EmptyPage());

        private static PaginatedResult<ConsumptionInterval> EmptyPage() => new();

        private static async IAsyncEnumerable<ConsumptionInterval> EmptyIntervals()
        {
            await Task.CompletedTask;
            yield break;
        }
    }

    private sealed class StubIndustryService : IIndustryService
    {
        public IAsyncEnumerable<GridSupplyPointLookup> ListGridSupplyPointsByPostcodeAsync(
            string postcode,
            CancellationToken cancellationToken = default)
        {
            return EmptyGridSupplyPoints();
        }

        public Task<ElectricityMeterPointLookup> GetElectricityMeterPointAsync(
            string mpan,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ElectricityMeterPointLookup());

        private static async IAsyncEnumerable<GridSupplyPointLookup> EmptyGridSupplyPoints()
        {
            await Task.CompletedTask;
            yield break;
        }
    }

    private sealed class StubProductService : IProductService
    {
        public IAsyncEnumerable<Product> ListAsync(
            ProductListRequest? request = null,
            CancellationToken cancellationToken = default)
        {
            return EmptyProducts();
        }

        public Task<ProductDetail> GetAsync(
            string productCode,
            DateTimeOffset? tariffsActiveAt = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ProductDetail());

        private static async IAsyncEnumerable<Product> EmptyProducts()
        {
            await Task.CompletedTask;
            yield break;
        }
    }

    private sealed class StubRateLimitService : IRateLimitService
    {
        public Task<RateLimitInfo> GetRateLimitInfoAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new RateLimitInfo());
    }

    private sealed class StubMeterReadingService : IMeterReadingService
    {
        public Task<ElectricityMeterReadingResult> CreateElectricityAsync(
            ElectricityMeterReadingCreate request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ElectricityMeterReadingResult());

        public Task<GasMeterReadingResult> CreateGasAsync(
            GasMeterReadingCreate request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new GasMeterReadingResult());

        public Task<ReadingConsentGranularity> GetReadingConsentGranularityAsync(
            string mpan,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ReadingConsentGranularity());
    }

    private sealed class StubViewerService : IViewerService
    {
        public Task<ViewerDetail> GetViewerAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(new ViewerDetail());
    }

    private sealed class StubDeviceService : IDeviceService
    {
        public Task<IReadOnlyList<SmartFlexDevice>> ListDevicesAsync(
            string accountNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<SmartFlexDevice>>([]);

        public Task<IReadOnlyList<FlexPlannedDispatch>> ListFlexPlannedDispatchesAsync(
            string deviceId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<FlexPlannedDispatch>>([]);
    }

    private sealed class StubProductSwitchService : IProductSwitchService
    {
        public Task<ProductSwitchResult> InitiateAsync(
            ProductSwitchInitiate request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new ProductSwitchResult
            {
                ProductCode = "E-1R-VAR-22-11-25-C",
                SwitchDate = new DateOnly(2026, 4, 1),
            });
    }

    private sealed class StubOctoplusService : IOctoplusService
    {
        public Task<OctoplusAccountInfo> GetAccountInfoAsync(
            string accountNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new OctoplusAccountInfo());

        public Task<IReadOnlyList<LoyaltyPointLedgerEntry>> ListLoyaltyPointLedgersAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LoyaltyPointLedgerEntry>>([]);

        public Task<LoyaltyPointsBalance> GetLoyaltyPointsBalanceAsync(
            string accountNumber,
            string? schemeCode = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new LoyaltyPointsBalance());

        public Task<WheelOfFortuneSpinsAvailability> GetWheelOfFortuneSpinsAsync(
            string accountNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new WheelOfFortuneSpinsAvailability());

        public Task<JoinOctoplusCampaignResult> JoinCampaignAsync(
            string accountNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new JoinOctoplusCampaignResult());

        public Task<RedeemLoyaltyPointsResult> RedeemPointsForAccountCreditAsync(
            string accountNumber,
            int points,
            string? schemeCode = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new RedeemLoyaltyPointsResult());
    }

    private sealed class StubSmartMeterTelemetryService : ISmartMeterTelemetryService
    {
        public Task<IReadOnlyList<SmartMeterDevice>> ListSmartDevicesAsync(
            string accountNumber,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<SmartMeterDevice>>([]);

        public Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
            string deviceId,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<SmartMeterTelemetryReading>>([]);

        public Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
            string deviceId,
            SmartMeterTelemetryRange range,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<SmartMeterTelemetryReading>>([]);
    }

    private sealed class StubAccountBillingService : IAccountBillingService
    {
        public Task<GraphQlRelayPage<AccountBill>> ListBillsPageAsync(
            string accountNumber,
            AccountBillListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new GraphQlRelayPage<AccountBill>());

        public Task<GraphQlRelayPage<AccountTransaction>> ListTransactionsPageAsync(
            string accountNumber,
            AccountTransactionListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new GraphQlRelayPage<AccountTransaction>());

        public Task<GraphQlRelayPage<AccountPayment>> ListPaymentsPageAsync(
            string accountNumber,
            AccountPaymentListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new GraphQlRelayPage<AccountPayment>());
    }

    private sealed class StubTariffRatesService : ITariffRatesService
    {
        public IAsyncEnumerable<TariffCharge> ListStandingChargesAsync(
            TariffCode tariffCode,
            TariffChargeListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyCharges();

        public IAsyncEnumerable<TariffCharge> ListStandardUnitRatesAsync(
            TariffCode tariffCode,
            TariffChargeListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyCharges();

        public IAsyncEnumerable<TariffCharge> ListDayUnitRatesAsync(
            TariffCode tariffCode,
            TariffChargeListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyCharges();

        public IAsyncEnumerable<TariffCharge> ListNightUnitRatesAsync(
            TariffCode tariffCode,
            TariffChargeListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyCharges();

        public IAsyncEnumerable<TariffCharge> ListChargesAsync(
            TariffCode tariffCode,
            TariffChargeKind chargeKind,
            TariffChargeListRequest? request = null,
            CancellationToken cancellationToken = default) =>
            EmptyCharges();

        private static async IAsyncEnumerable<TariffCharge> EmptyCharges()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
