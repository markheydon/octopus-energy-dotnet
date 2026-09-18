namespace OctopusEnergy.Client.Models.Products;

/// <summary>
/// Optional filters for <c>GET /v1/products/</c>.
/// </summary>
public sealed class ProductListRequest
{
    /// <summary>
    /// Filter by retail brand (for example <c>OCTOPUS_ENERGY</c>).
    /// </summary>
    public string? Brand { get; init; }

    /// <summary>
    /// Filter by variable products when set.
    /// </summary>
    public bool? IsVariable { get; init; }

    /// <summary>
    /// Filter by green products when set.
    /// </summary>
    public bool? IsGreen { get; init; }

    /// <summary>
    /// Filter by tracker products when set.
    /// </summary>
    public bool? IsTracker { get; init; }

    /// <summary>
    /// Filter by prepayment products when set.
    /// </summary>
    public bool? IsPrepay { get; init; }

    /// <summary>
    /// Filter by business products when set.
    /// </summary>
    public bool? IsBusiness { get; init; }

    /// <summary>
    /// Show products that were available at the given instant.
    /// </summary>
    public DateTimeOffset? AvailableAt { get; init; }
}
