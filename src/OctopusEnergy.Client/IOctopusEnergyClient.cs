using OctopusEnergy.Client.Services.AccountBilling;
using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
using OctopusEnergy.Client.Services.Devices;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.MeterReadings;
using OctopusEnergy.Client.Services.Products;
using OctopusEnergy.Client.Services.RateLimits;
using OctopusEnergy.Client.Services.Viewer;

namespace OctopusEnergy.Client;

/// <summary>
/// Customer-facing client for the public Octopus Energy (Kraken) APIs.
/// </summary>
/// <remarks>
/// Resource services share transport, pagination, and error handling implemented by
/// <see cref="OctopusEnergyClient"/>. Catalogue, account, consumption, and industry
/// calls use REST. Curated customer GraphQL (for example
/// <see cref="Viewer"/>, <see cref="RateLimits"/>, <see cref="MeterReadings"/>,
/// <see cref="AccountBilling"/>, and <see cref="Devices"/>)
/// uses the same client and dashboard API key; JWT exchange is internal. Construct an
/// <see cref="OctopusEnergyClient"/> to call the live API; depend on this interface
/// in application code so unit tests can substitute behaviour.
/// </remarks>
public interface IOctopusEnergyClient : IDisposable
{
    /// <summary>
    /// Customer account detail.
    /// </summary>
    IAccountService Accounts { get; }

    /// <summary>
    /// Electricity and gas consumption intervals.
    /// </summary>
    IConsumptionService Consumption { get; }

    /// <summary>
    /// Public industry lookups (postcode GSP and MPAN metadata).
    /// </summary>
    IIndustryService Industry { get; }

    /// <summary>
    /// Product catalogue and product detail.
    /// </summary>
    IProductService Products { get; }

    /// <summary>
    /// Standing charges and unit rates for product tariffs.
    /// </summary>
    ITariffRatesService TariffRates { get; }

    /// <summary>
    /// Kraken hourly points allowance and related rate-limit information.
    /// </summary>
    IRateLimitService RateLimits { get; }

    /// <summary>
    /// GraphQL meter reading mutations for the authenticated API key.
    /// </summary>
    IMeterReadingService MeterReadings { get; }

    /// <summary>
    /// GraphQL viewer discovery for account numbers linked to the API key.
    /// </summary>
    IViewerService Viewer { get; }

    /// <summary>
    /// GraphQL account bills, ledger transactions, and payments for the authenticated API key.
    /// </summary>
    IAccountBillingService AccountBilling { get; }

    /// <summary>
    /// GraphQL SmartFlex device discovery and planned dispatch windows for the authenticated API key.
    /// </summary>
    IDeviceService Devices { get; }
}
