using OctopusEnergy.Client.Infrastructure.Http;
using OctopusEnergy.Client.Models.Industry;

namespace OctopusEnergy.Client.Services.Industry;

/// <summary>
/// Public industry lookups (postcode GSP and electricity meter-point metadata).
/// </summary>
public sealed class IndustryService
{
    private const string GridSupplyPointsPath = "industry/grid-supply-points/";
    private const string ElectricityMeterPointsPathPrefix = "electricity-meter-points/";

    private readonly RestClient _rest;

    internal IndustryService(RestClient rest)
    {
        ArgumentNullException.ThrowIfNull(rest);
        _rest = rest;
    }

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
    public IAsyncEnumerable<GridSupplyPointLookup> ListGridSupplyPointsByPostcodeAsync(
        string postcode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(postcode))
        {
            throw new OctopusEnergyRequestException("Postcode is required.");
        }

        List<RestQuery.QueryParameter> parameters =
        [
            new RestQuery.QueryParameter("postcode", postcode.Trim()),
        ];

        string path = RestQuery.Append(GridSupplyPointsPath, parameters);
        return _rest.GetAllPagesAsync<GridSupplyPointLookup>(path, cancellationToken);
    }

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
    public Task<ElectricityMeterPointLookup> GetElectricityMeterPointAsync(
        string mpan,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(mpan))
        {
            throw new OctopusEnergyRequestException("MPAN is required.");
        }

        string path = $"{ElectricityMeterPointsPathPrefix}{Uri.EscapeDataString(mpan)}/";
        return _rest.GetAsync<ElectricityMeterPointLookup>(path, cancellationToken);
    }
}
