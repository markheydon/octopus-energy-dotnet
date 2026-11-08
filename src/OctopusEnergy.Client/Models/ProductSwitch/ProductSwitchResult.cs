namespace OctopusEnergy.Client.Models.ProductSwitch;

/// <summary>
/// Outcome of <c>initiateProductSwitch</c> when the mutation succeeds without GraphQL errors.
/// </summary>
public sealed class ProductSwitchResult
{
    /// <summary>
    /// Gets the tariff product code for the switched agreement.
    /// </summary>
    /// <remarks>
    /// Where the code matches the UK encoding documented for REST tariffs, use <see cref="TariffCode.TryParse"/>.
    /// See <see href="https://github.com/markheydon/octopus-energy-dotnet/blob/main/docs/how-to/tariff-codes.md">tariff codes</see>.
    /// </remarks>
    public required string ProductCode { get; init; }

    /// <summary>
    /// Gets the effective switch date returned by Kraken.
    /// </summary>
    public required DateOnly SwitchDate { get; init; }
}
