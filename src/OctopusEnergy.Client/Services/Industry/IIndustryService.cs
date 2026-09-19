using OctopusEnergy.Client.Models.Industry;

namespace OctopusEnergy.Client.Services.Industry;

/// <summary>
/// Public industry lookups (postcode GSP and electricity meter-point metadata).
/// </summary>
public interface IIndustryService
{
    /// <summary>
    /// Lists grid supply points for a UK postcode.
    /// </summary>
    /// <param name="postcode">UK postcode (spaces optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Matching grid supply point records.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="postcode"/> is null or whitespace.
    /// </exception>
    /// <exception cref="OctopusEnergyApiException">
    /// Thrown when the API returns an error response.
    /// </exception>
    IAsyncEnumerable<GridSupplyPointLookup> ListGridSupplyPointsByPostcodeAsync(
        string postcode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves GSP and profile class for an electricity MPAN.
    /// </summary>
    /// <param name="mpan">13-digit MPAN.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Meter-point industry metadata.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="mpan"/> is null or whitespace.
    /// </exception>
    /// <exception cref="OctopusEnergyApiException">
    /// Thrown when the API returns an error response (for example HTTP 404 for an unknown MPAN).
    /// </exception>
    Task<ElectricityMeterPointLookup> GetElectricityMeterPointAsync(
        string mpan,
        CancellationToken cancellationToken = default);
}
