using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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

    internal async Task<GraphQlResponseEnvelope<TData>> PostAsync<TData>(
        string query,
        object? variables,
        string? authorizationHeaderValue,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        GraphQlRequestEnvelope body = new()
        {
            Query = query,
            Variables = variables,
        };

        using HttpRequestMessage request = new(HttpMethod.Post, _graphQlEndpoint)
        {
            Content = JsonContent.Create(body, options: OctopusJsonSerializerOptions.Default),
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
            envelope = await JsonSerializer.DeserializeAsync<GraphQlResponseEnvelope<TData>>(
                contentStream,
                OctopusJsonSerializerOptions.Default,
                cancellationToken).ConfigureAwait(false);
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
}
