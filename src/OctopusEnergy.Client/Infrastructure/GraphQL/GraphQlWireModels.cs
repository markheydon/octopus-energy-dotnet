using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class GraphQlRequestEnvelope
{
    [JsonPropertyName("query")]
    public string Query { get; init; } = string.Empty;

    [JsonPropertyName("variables")]
    public object? Variables { get; init; }
}

internal sealed class GraphQlResponseEnvelope<TData>
{
    [JsonPropertyName("data")]
    public TData? Data { get; init; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<GraphQlError>? Errors { get; init; }
}

internal sealed class GraphQlError
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("extensions")]
    public GraphQlErrorExtensions? Extensions { get; init; }
}

internal sealed class GraphQlErrorExtensions
{
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; init; }
}

internal sealed class ObtainKrakenTokenResponseData
{
    [JsonPropertyName("obtainKrakenToken")]
    public ObtainKrakenTokenResult? ObtainKrakenToken { get; init; }
}

internal sealed class ObtainKrakenTokenResult
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; init; }

    [JsonPropertyName("refreshExpiresIn")]
    public long? RefreshExpiresIn { get; init; }
}

internal sealed class ObtainKrakenTokenVariables
{
    [JsonPropertyName("input")]
    public ObtainKrakenTokenInput Input { get; init; } = new();
}

internal sealed class ObtainKrakenTokenInput
{
    [JsonPropertyName("APIKey")]
    public string? APIKey { get; init; }

    [JsonPropertyName("refreshToken")]
    public string? RefreshToken { get; init; }
}
