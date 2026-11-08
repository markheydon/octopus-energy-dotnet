using OctopusEnergy.Client.Models.ProductSwitch;

namespace OctopusEnergy.Client.Services.ProductSwitch;

/// <summary>
/// GraphQL product switch mutation for the authenticated dashboard API key.
/// </summary>
public interface IProductSwitchService
{
    /// <summary>
    /// Initiates a product switch for the given account.
    /// </summary>
    /// <param name="request">Switch details. Provide exactly one of <see cref="ProductSwitchInitiate.QuotedProductId"/> or <see cref="ProductSwitchInitiate.OfferIdentifier"/>.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The accepted switch details from Kraken.</returns>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when the client was created without a dashboard API key or request fields fail local validation.
    /// </exception>
    /// <exception cref="OctopusEnergyKrakenException">
    /// Thrown when the GraphQL API returns <c>errors.extensions.errorCode</c> (for example insufficient permission or an invalid quote).
    /// </exception>
    /// <exception cref="OctopusEnergyParseException">
    /// Thrown when the response cannot be deserialised or is missing expected data.
    /// </exception>
    Task<ProductSwitchResult> InitiateAsync(
        ProductSwitchInitiate request,
        CancellationToken cancellationToken = default);
}
