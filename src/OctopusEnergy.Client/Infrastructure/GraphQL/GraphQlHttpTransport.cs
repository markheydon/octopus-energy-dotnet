using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class GraphQlHttpTransport
{
    private readonly HttpClient _httpClient;
    private readonly Uri _graphQlEndpoint;

    internal GraphQlHttpTransport(HttpClient httpClient, Uri graphQlEndpoint)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(graphQlEndpoint);

        _httpClient = httpClient;
        _graphQlEndpoint = graphQlEndpoint;
    }

    internal Task<GraphQlResponseEnvelope<TData>> PostAsync<TData>(
        string query,
        string? authorizationHeaderValue,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        GraphQlQueryOnlyEnvelope body = new() { Query = query };
        JsonTypeInfo typeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlQueryOnlyEnvelope))
            ?? throw new InvalidOperationException("GraphQlQueryOnlyEnvelope is not registered for source generation.");

        return PostCoreAsync<TData>(body, typeInfo, authorizationHeaderValue, cancellationToken);
    }

    internal Task<GraphQlResponseEnvelope<TData>> PostAsync<TData, TVariables>(
        string query,
        TVariables? variables,
        string? authorizationHeaderValue,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        GraphQlRequestEnvelope<TVariables> body = new()
        {
            Query = query,
            Variables = variables,
        };

        JsonTypeInfo typeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlRequestEnvelope<TVariables>))
            ?? throw new InvalidOperationException(
                $"GraphQlRequestEnvelope<{typeof(TVariables).Name}> is not registered for source generation.");

        return PostCoreAsync<TData>(body, typeInfo, authorizationHeaderValue, cancellationToken);
    }

    private async Task<GraphQlResponseEnvelope<TData>> PostCoreAsync<TData>(
        object requestBody,
        JsonTypeInfo requestTypeInfo,
        string? authorizationHeaderValue,
        CancellationToken cancellationToken)
    {
        string requestJson = JsonSerializer.Serialize(requestBody, requestTypeInfo);
        using HttpRequestMessage request = new(HttpMethod.Post, _graphQlEndpoint)
        {
            Content = new StringContent(requestJson, Encoding.UTF8, "application/json"),
        };

        GraphQlRequestHeaders.Apply(request, authorizationHeaderValue);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken).ConfigureAwait(false);

        await using Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);

        GraphQlResponseEnvelope<TData>? envelope;

        try
        {
            JsonTypeInfo responseTypeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlResponseEnvelope<TData>))
                ?? throw new InvalidOperationException(
                    $"GraphQlResponseEnvelope<{typeof(TData).Name}> is not registered for source generation.");

            envelope = await JsonSerializer.DeserializeAsync(
                contentStream,
                responseTypeInfo,
                cancellationToken).ConfigureAwait(false) as GraphQlResponseEnvelope<TData>;
        }
        catch (JsonException ex)
        {
            throw new OctopusEnergyParseException("The GraphQL API returned a response that could not be deserialised.", ex);
        }

        if (envelope is null)
        {
            throw new OctopusEnergyParseException("The GraphQL API returned an empty response body.");
        }

        if (!response.IsSuccessStatusCode)
        {
            string? message = envelope.Errors is { Count: > 0 }
                ? envelope.Errors[0].Message
                : null;

            string detail = !string.IsNullOrWhiteSpace(message)
                ? message!
                : $"The Octopus GraphQL API returned HTTP {(int)response.StatusCode} ({response.StatusCode}).";
            throw new OctopusEnergyApiException(response.StatusCode, detail);
        }

        return envelope;
    }

    /// <summary>
    /// Live allow-list probe path. Variable payloads may be anonymous types; only tests call this.
    /// </summary>
    internal Task<GraphQlResponseEnvelope<JsonElement>> PostProbeAsync(
        string query,
        object? variables,
        string? authorizationHeaderValue,
        JsonSerializerOptions variableSerializationOptions,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(variableSerializationOptions);

        JsonTypeInfo requestTypeInfo;
        object requestBody;

        if (variables is null)
        {
            requestBody = new GraphQlQueryOnlyEnvelope { Query = query };
            requestTypeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlQueryOnlyEnvelope))
                ?? throw new InvalidOperationException("GraphQlQueryOnlyEnvelope is not registered for source generation.");
        }
        else
        {
            JsonElement variablesElement = JsonSerializer.SerializeToElement(variables, variableSerializationOptions);
            requestBody = new GraphQlRequestEnvelope<JsonElement>
            {
                Query = query,
                Variables = variablesElement,
            };

            requestTypeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(GraphQlRequestEnvelope<JsonElement>))
                ?? throw new InvalidOperationException("GraphQlRequestEnvelope<JsonElement> is not registered for source generation.");
        }

        return PostCoreAsync<JsonElement>(requestBody, requestTypeInfo, authorizationHeaderValue, cancellationToken);
    }
}
