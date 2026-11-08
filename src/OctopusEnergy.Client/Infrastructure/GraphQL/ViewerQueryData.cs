using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class ViewerQueryData
{
    [JsonPropertyName("viewer")]
    public ViewerWire? Viewer { get; init; }
}

internal sealed class ViewerWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("accounts")]
    public IReadOnlyList<ViewerAccountWire>? Accounts { get; init; }

    [JsonPropertyName("portfolios")]
    public IReadOnlyList<ViewerPortfolioWire>? Portfolios { get; init; }
}

internal sealed class ViewerAccountWire
{
    [JsonPropertyName("number")]
    public string? Number { get; init; }
}

internal sealed class ViewerPortfolioWire
{
    [JsonPropertyName("accounts")]
    public IReadOnlyList<ViewerAccountWire>? Accounts { get; init; }
}
