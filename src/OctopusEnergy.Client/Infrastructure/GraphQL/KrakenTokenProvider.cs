namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class KrakenTokenProvider : IDisposable
{
    private static readonly TimeSpan AccessExpirySkew = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan FallbackAccessLifetime = TimeSpan.FromMinutes(55);

    private readonly GraphQlHttpTransport _transport;
    private readonly string _apiKey;
    private readonly SemaphoreSlim _refreshLock = new(1, 1);

    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset? _accessExpiresAtUtc;
    private DateTimeOffset? _refreshExpiresAtUtc;

    internal KrakenTokenProvider(GraphQlHttpTransport transport, string apiKey)
    {
        ArgumentNullException.ThrowIfNull(transport);
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

        _transport = transport;
        _apiKey = apiKey;
    }

    internal async Task<string> GetAuthorizationHeaderValueAsync(CancellationToken cancellationToken)
    {
        if (IsAccessTokenValid())
        {
            return KrakenJwtAuthorizationHeader.Format(_accessToken!);
        }

        await _refreshLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsAccessTokenValid())
            {
                return KrakenJwtAuthorizationHeader.Format(_accessToken!);
            }

            if (IsRefreshTokenValid())
            {
                await RefreshInternalAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await ObtainWithApiKeyInternalAsync(cancellationToken).ConfigureAwait(false);
            }

            return KrakenJwtAuthorizationHeader.Format(_accessToken!);
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    internal Task<string> ForceRefreshAsync(CancellationToken cancellationToken)
    {
        _accessToken = null;
        _accessExpiresAtUtc = null;

        return GetAuthorizationHeaderValueAsync(cancellationToken);
    }

    public void Dispose()
    {
        _refreshLock.Dispose();
    }

    private bool IsAccessTokenValid()
    {
        return _accessToken is not null
            && _accessExpiresAtUtc is not null
            && _accessExpiresAtUtc.Value > DateTimeOffset.UtcNow.Add(AccessExpirySkew);
    }

    private bool IsRefreshTokenValid()
    {
        return _refreshToken is not null
            && _refreshExpiresAtUtc is not null
            && _refreshExpiresAtUtc.Value > DateTimeOffset.UtcNow.Add(AccessExpirySkew);
    }

    private async Task ObtainWithApiKeyInternalAsync(CancellationToken cancellationToken)
    {
        ObtainKrakenTokenVariables variables = new()
        {
            Input = new ObtainKrakenTokenInput { APIKey = _apiKey },
        };

        GraphQlResponseEnvelope<ObtainKrakenTokenResponseData> response = await _transport.PostAsync<ObtainKrakenTokenResponseData>(
            KrakenTokenMutations.ObtainKrakenToken,
            variables,
            authorizationHeaderValue: null,
            cancellationToken).ConfigureAwait(false);

        ApplyObtainResult(response);
    }

    private async Task RefreshInternalAsync(CancellationToken cancellationToken)
    {
        ObtainKrakenTokenVariables variables = new()
        {
            Input = new ObtainKrakenTokenInput { RefreshToken = _refreshToken },
        };

        GraphQlResponseEnvelope<ObtainKrakenTokenResponseData> response = await _transport.PostAsync<ObtainKrakenTokenResponseData>(
            KrakenTokenMutations.ObtainKrakenToken,
            variables,
            authorizationHeaderValue: null,
            cancellationToken).ConfigureAwait(false);

        ApplyObtainResult(response);
    }

    private void ApplyObtainResult(GraphQlResponseEnvelope<ObtainKrakenTokenResponseData> response)
    {
        if (response.Errors is { Count: > 0 })
        {
            throw CreateGraphQlErrorException(response.Errors);
        }

        ObtainKrakenTokenResult? result = response.Data?.ObtainKrakenToken
            ?? throw new OctopusEnergyParseException("The obtainKrakenToken mutation returned no data.");

        ArgumentException.ThrowIfNullOrWhiteSpace(result.Token);

        _accessToken = result.Token;
        _refreshToken = result.RefreshToken;
        _accessExpiresAtUtc = KrakenJwtExpiry.TryReadExpiryUtc(result.Token)
            ?? DateTimeOffset.UtcNow.Add(FallbackAccessLifetime);

        if (result.RefreshExpiresIn is long refreshExpiresIn)
        {
            _refreshExpiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(refreshExpiresIn);
        }
        else
        {
            _refreshExpiresAtUtc = null;
        }
    }

    private static OctopusEnergyApiException CreateGraphQlErrorException(IReadOnlyList<GraphQlError> errors)
    {
        GraphQlError first = errors[0];
        string? code = first.Extensions?.ErrorCode;
        string? message = first.Message;

        string detail = code is not null
            ? $"{code}: {message ?? "The GraphQL API returned an error."}"
            : message ?? "The GraphQL API returned an error.";

        return new OctopusEnergyApiException(System.Net.HttpStatusCode.OK, detail);
    }
}
