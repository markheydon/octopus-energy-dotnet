namespace OctopusEnergy.Client.Models.Common;

/// <summary>
/// A single page from a REST list endpoint.
/// </summary>
/// <typeparam name="TItem">The resource type in <see cref="Results"/>.</typeparam>
public sealed class PaginatedResult<TItem>
{
    /// <summary>
    /// Total number of items matching the query across all pages.
    /// </summary>
    public int Count { get; init; }

    /// <summary>
    /// URL of the next page, or <see langword="null"/> when this is the last page.
    /// </summary>
    public string? Next { get; init; }

    /// <summary>
    /// URL of the previous page, or <see langword="null"/> when this is the first page.
    /// </summary>
    public string? Previous { get; init; }

    /// <summary>
    /// Items returned on this page.
    /// </summary>
    public IReadOnlyList<TItem> Results { get; init; } = [];
}
