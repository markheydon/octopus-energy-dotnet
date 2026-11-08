namespace OctopusEnergy.Client.Models.Viewer;

/// <summary>
/// The authenticated Kraken viewer for the dashboard API key, including discoverable account numbers.
/// </summary>
/// <remarks>
/// Account numbers from direct <c>viewer.accounts</c> and nested <c>viewer.portfolios.accounts</c> are
/// merged into <see cref="AccountNumbers"/> without duplicates. The SDK does not choose a default account
/// when multiple numbers are returned.
/// </remarks>
public sealed class ViewerDetail
{
    /// <summary>
    /// Gets the viewer identifier when the API returns one.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Gets distinct account numbers linked to this viewer, in discovery order
    /// (direct accounts first, then portfolio-only accounts).
    /// </summary>
    public IReadOnlyList<string> AccountNumbers { get; init; } = [];
}
