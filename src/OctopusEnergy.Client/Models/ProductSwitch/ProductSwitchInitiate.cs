namespace OctopusEnergy.Client.Models.ProductSwitch;

/// <summary>
/// Input for initiating a product switch via GraphQL.
/// </summary>
public sealed class ProductSwitchInitiate
{
    /// <summary>
    /// Gets the account number requesting the switch (for example <c>A-12345678</c>).
    /// </summary>
    public required string AccountNumber { get; init; }

    /// <summary>
    /// Gets the civil date on which the switch becomes effective.
    /// </summary>
    public required DateOnly SwitchDate { get; init; }

    /// <summary>
    /// Gets the quoted product id from a supply-point quote.
    /// Provide this or <see cref="OfferIdentifier"/>, not both.
    /// </summary>
    public string? QuotedProductId { get; init; }

    /// <summary>
    /// Gets the commercial offer identifier to accept.
    /// Provide this or <see cref="QuotedProductId"/>, not both.
    /// </summary>
    public string? OfferIdentifier { get; init; }

    /// <summary>
    /// Gets whether product-switch communications are suppressed.
    /// When null, Kraken applies its default (<c>true</c>).
    /// </summary>
    public bool? SuppressCommunications { get; init; }
}
