namespace OctopusEnergy.Client.Models.Common;

/// <summary>
/// A single page from a Kraken GraphQL Relay connection.
/// </summary>
/// <typeparam name="TItem">The resource type in <see cref="Results"/>.</typeparam>
public sealed class GraphQlRelayPage<TItem>
{
    /// <summary>
    /// Items returned on this page.
    /// </summary>
    public IReadOnlyList<TItem> Results { get; init; } = [];

    /// <summary>
    /// Cursor for the last edge on this page, or <see langword="null"/> when the page is empty.
    /// </summary>
    public string? EndCursor { get; init; }

    /// <summary>
    /// Cursor for the first edge on this page, or <see langword="null"/> when the page is empty.
    /// </summary>
    public string? StartCursor { get; init; }

    /// <summary>
    /// Whether Kraken reports a further page when paging forward.
    /// </summary>
    public bool HasNextPage { get; init; }

    /// <summary>
    /// Whether Kraken reports a previous page when paging backward.
    /// </summary>
    /// <remarks>
    /// When you supplied an <c>after</c> cursor for forward paging, see
    /// <see cref="GraphQlRelayPageInfo.ApplyForwardPagingRules(bool, bool)"/> for Kraken Relay behaviour.
    /// </remarks>
    public bool HasPreviousPage { get; init; }
}
