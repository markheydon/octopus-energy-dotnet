using System.Net;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class GraphQlClient : IDisposable
{
    internal const string ExpiredTokenErrorCode = "KT-CT-1112";

    private readonly GraphQlHttpTransport _transport;
    private readonly KrakenTokenProvider _tokenProvider;

    internal GraphQlClient(GraphQlHttpTransport transport, KrakenTokenProvider tokenProvider)
    {
        ArgumentNullException.ThrowIfNull(transport);
        ArgumentNullException.ThrowIfNull(tokenProvider);

        _transport = transport;
        _tokenProvider = tokenProvider;
    }

    public void Dispose()
    {
        _tokenProvider.Dispose();
    }

    internal async Task<TData> ExecuteAsync<TData>(
        string query,
        object? variables,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);

        string authorization = await _tokenProvider.GetAuthorizationHeaderValueAsync(cancellationToken)
            .ConfigureAwait(false);

        GraphQlResponseEnvelope<TData> response = await _transport.PostAsync<TData>(
            query,
            variables,
            authorization,
            cancellationToken).ConfigureAwait(false);

        if (ContainsErrorCode(response.Errors, ExpiredTokenErrorCode))
        {
            authorization = await _tokenProvider.ForceRefreshAsync(cancellationToken).ConfigureAwait(false);

            response = await _transport.PostAsync<TData>(
                query,
                variables,
                authorization,
                cancellationToken).ConfigureAwait(false);
        }

        if (response.Errors is { Count: > 0 })
        {
            throw GraphQlKrakenErrors.Map(response.Errors);
        }

        return response.Data ?? throw new OctopusEnergyParseException("The GraphQL API returned JSON null where a response model was expected.");
    }

    private static bool ContainsErrorCode(IReadOnlyList<GraphQlError>? errors, string errorCode)
    {
        return errors?.Any(error =>
            string.Equals(error.Extensions?.ErrorCode, errorCode, StringComparison.Ordinal)) == true;
    }

}
