using System.Globalization;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.ProductSwitch;

namespace OctopusEnergy.Client.Services.ProductSwitch;

/// <summary>
/// GraphQL product switch mutation for the authenticated dashboard API key.
/// </summary>
public sealed class ProductSwitchService : IProductSwitchService
{
    private readonly GraphQlClient? _graphQl;

    internal ProductSwitchService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<ProductSwitchResult> InitiateAsync(
        ProductSwitchInitiate request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        EnsureGraphQl();
        ValidateRequest(request);

        InitiateProductSwitchMutationVariables variables = new()
        {
            AccountNumber = request.AccountNumber.Trim(),
            SwitchDate = FormatSwitchDate(request.SwitchDate),
            QuotedProductId = string.IsNullOrWhiteSpace(request.QuotedProductId)
                ? null
                : request.QuotedProductId.Trim(),
            OfferIdentifier = string.IsNullOrWhiteSpace(request.OfferIdentifier)
                ? null
                : request.OfferIdentifier.Trim(),
            SuppressCommunications = request.SuppressCommunications,
        };

        InitiateProductSwitchMutationData data = await _graphQl!.ExecuteAsync<InitiateProductSwitchMutationData, InitiateProductSwitchMutationVariables>(
            ProductSwitchMutations.Initiate,
            variables,
            cancellationToken).ConfigureAwait(false);

        InitiateProductSwitchWire payload = data.InitiateProductSwitch
            ?? throw new OctopusEnergyParseException(
                "The initiate product switch mutation returned no data.");

        return MapResult(payload);
    }

    private static void ValidateRequest(ProductSwitchInitiate request)
    {
        if (string.IsNullOrWhiteSpace(request.AccountNumber))
        {
            throw new OctopusEnergyRequestException("Account number is required.");
        }

        bool hasQuotedProductId = !string.IsNullOrWhiteSpace(request.QuotedProductId);
        bool hasOfferIdentifier = !string.IsNullOrWhiteSpace(request.OfferIdentifier);

        if (hasQuotedProductId == hasOfferIdentifier)
        {
            throw new OctopusEnergyRequestException(
                "Provide exactly one of quoted product id or offer identifier.");
        }
    }

    private static ProductSwitchResult MapResult(InitiateProductSwitchWire payload)
    {
        if (string.IsNullOrWhiteSpace(payload.ProductCode))
        {
            throw new OctopusEnergyParseException(
                "The initiate product switch mutation did not return a product code.");
        }

        if (string.IsNullOrWhiteSpace(payload.SwitchDate))
        {
            throw new OctopusEnergyParseException(
                "The initiate product switch mutation did not return a switch date.");
        }

        if (!DateOnly.TryParse(payload.SwitchDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly switchDate))
        {
            throw new OctopusEnergyParseException(
                "The initiate product switch mutation returned an unrecognised switch date.");
        }

        return new ProductSwitchResult
        {
            ProductCode = payload.ProductCode.Trim(),
            SwitchDate = switchDate,
        };
    }

    private static string FormatSwitchDate(DateOnly switchDate)
    {
        return switchDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private void EnsureGraphQl()
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to initiate a product switch via GraphQL.");
        }
    }
}
