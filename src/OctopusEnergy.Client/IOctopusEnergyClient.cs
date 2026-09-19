using OctopusEnergy.Client.Services.Accounts;
using OctopusEnergy.Client.Services.Consumption;
using OctopusEnergy.Client.Services.Industry;
using OctopusEnergy.Client.Services.Products;

namespace OctopusEnergy.Client;

/// <summary>
/// Customer-facing client for the public Octopus Energy (Kraken) APIs.
/// </summary>
/// <remarks>
/// Resource services use the shared REST transport, pagination, and error handling
/// implemented by <see cref="OctopusEnergyClient"/>. Construct an
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
}
