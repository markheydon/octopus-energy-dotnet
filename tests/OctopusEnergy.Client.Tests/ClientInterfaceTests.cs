using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Consumption;
using OctopusEnergy.Client.Models.Industry;
using OctopusEnergy.Client.Models.Products;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.Products;

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
        }

        public string? LastRequestedAccountNumber { get; private set; }

        public IAccountService Accounts { get; }

        public IConsumptionService Consumption { get; }

        public IIndustryService Industry { get; }

        public IProductService Products { get; }

        public ITariffRatesService TariffRates { get; }

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
