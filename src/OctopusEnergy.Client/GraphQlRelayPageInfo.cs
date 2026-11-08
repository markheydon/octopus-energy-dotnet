namespace OctopusEnergy.Client;

/// <summary>
/// Relay connection paging rules documented for Kraken GraphQL list operations.
/// </summary>
public static class GraphQlRelayPageInfo
{
    /// <summary>
    /// Applies Kraken Relay paging rules to <paramref name="hasPreviousPage"/>.
    /// </summary>
    /// <param name="hasPreviousPage">The <c>pageInfo.hasPreviousPage</c> value from the API.</param>
    /// <param name="pagingWithAfter">
    /// <see langword="true"/> when this request used an <c>after</c> cursor for forward paging.
    /// </param>
    /// <returns>
    /// <see langword="false"/> when paging forward with <c>after</c> (spec: <c>hasPreviousPage</c> is always false).
    /// Otherwise returns <paramref name="hasPreviousPage"/> unchanged.
    /// </returns>
    public static bool ApplyForwardPagingRules(bool hasPreviousPage, bool pagingWithAfter)
    {
        return pagingWithAfter ? false : hasPreviousPage;
    }
}
