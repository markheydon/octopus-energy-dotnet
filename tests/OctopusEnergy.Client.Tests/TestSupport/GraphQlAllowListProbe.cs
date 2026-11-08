using System.Text.Json;
using System.Text.RegularExpressions;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Configuration;
using OctopusEnergy.Client.Infrastructure.GraphQL;

namespace OctopusEnergy.Client.Tests.TestSupport;

internal enum GraphQlProbeOutcome
{
    Success,
    GraphQlError,
    HttpError,
    TokenObtainFailed,
    Skipped,
}

internal sealed class GraphQlAllowListProbeResult
{
    internal required string Operation { get; init; }

    internal GraphQlProbeOutcome Outcome { get; init; }

    internal IReadOnlyList<string> ErrorCodes { get; init; } = [];

    internal string? RedactedJson { get; init; }

    internal string? Note { get; init; }

    internal string? DiscoveredAccountNumber { get; init; }

    internal GraphQlAllowListMeterContext? DiscoveredMeterContext { get; init; }

    internal string? DiscoveredDeviceId { get; init; }
}

/// <summary>
/// Executes minimal GraphQL probes against the live API for allow-list documentation.
/// </summary>
internal sealed partial class GraphQlAllowListProbe : IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly GraphQlHttpTransport _transport;
    private readonly KrakenTokenProvider _tokenProvider;
    private readonly bool _ownsHttpClient;

    private GraphQlAllowListProbe(
        HttpClient httpClient,
        GraphQlHttpTransport transport,
        KrakenTokenProvider tokenProvider,
        bool ownsHttpClient)
    {
        _httpClient = httpClient;
        _transport = transport;
        _tokenProvider = tokenProvider;
        _ownsHttpClient = ownsHttpClient;
    }

    internal static GraphQlAllowListProbe Create(string apiKey)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = HttpClientConfiguration.NormalizeBaseAddress(new Uri(OctopusEnergyClient.DefaultBaseUrl)),
        };

        Uri graphQlEndpoint = GraphQlEndpoint.FromRestBaseAddress(httpClient.BaseAddress!);
        GraphQlHttpTransport transport = new(httpClient, graphQlEndpoint);
        KrakenTokenProvider tokenProvider = new(transport, apiKey);

        return new GraphQlAllowListProbe(httpClient, transport, tokenProvider, ownsHttpClient: true);
    }

    internal async Task<bool> TryEnsureTokenAsync(CancellationToken cancellationToken)
    {
        try
        {
            _ = await _tokenProvider.GetAuthorizationHeaderValueAsync(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (OctopusEnergyApiException ex) when (ex.Detail.Contains("KT-CT-11334", StringComparison.Ordinal))
        {
            return false;
        }
    }

    internal async Task<GraphQlAllowListProbeResult> ProbeAsync(
        string operation,
        string query,
        object? variables,
        CancellationToken cancellationToken,
        string? note = null)
    {
        string? authorization;
        try
        {
            authorization = await _tokenProvider.GetAuthorizationHeaderValueAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (OctopusEnergyApiException ex)
        {
            string? code = ExtractErrorCode(ex.Detail);
            return new GraphQlAllowListProbeResult
            {
                Operation = operation,
                Outcome = GraphQlProbeOutcome.TokenObtainFailed,
                ErrorCodes = code is not null ? [code] : [],
                Note = ex.Detail,
            };
        }
        catch (OctopusEnergyHttpException ex)
        {
            return new GraphQlAllowListProbeResult
            {
                Operation = operation,
                Outcome = GraphQlProbeOutcome.HttpError,
                Note = ex.Message,
            };
        }

        try
        {
            GraphQlResponseEnvelope<JsonElement> response = await _transport.PostAsync<JsonElement>(
                query,
                variables,
                authorization,
                cancellationToken).ConfigureAwait(false);

            IReadOnlyList<string> codes = response.Errors?
                .Select(error => error.Extensions?.ErrorCode)
                .Where(code => !string.IsNullOrWhiteSpace(code))
                .Select(code => code!)
                .ToList()
                ?? [];

            string? accountNumber = response.Data is { ValueKind: JsonValueKind.Object } data
                ? TryReadFirstAccountNumber(data)
                : null;

            GraphQlAllowListMeterContext? meterContext = response.Data is { ValueKind: JsonValueKind.Object } meterData
                ? GraphQlAllowListMeterContext.TryReadFromAccountMeterPoints(meterData)
                : null;

            string? deviceId = response.Data is { ValueKind: JsonValueKind.Object } deviceData
                ? TryReadFirstDeviceId(deviceData)
                : null;

            string rawJson = GraphQlProbeJsonRedactor.SerializeEnvelope(response);
            string redacted = GraphQlProbeJsonRedactor.RedactAndTruncate(rawJson);

            if (response.Errors is { Count: > 0 })
            {
                return new GraphQlAllowListProbeResult
                {
                    Operation = operation,
                    Outcome = GraphQlProbeOutcome.GraphQlError,
                    ErrorCodes = codes,
                    RedactedJson = redacted,
                    Note = note,
                    DiscoveredAccountNumber = accountNumber,
                    DiscoveredMeterContext = meterContext,
                    DiscoveredDeviceId = deviceId,
                };
            }

            return new GraphQlAllowListProbeResult
            {
                Operation = operation,
                Outcome = GraphQlProbeOutcome.Success,
                RedactedJson = redacted,
                Note = note,
                DiscoveredAccountNumber = accountNumber,
                DiscoveredMeterContext = meterContext,
                DiscoveredDeviceId = deviceId,
            };
        }
        catch (OctopusEnergyApiException ex)
        {
            string? code = ExtractErrorCode(ex.Detail);
            return new GraphQlAllowListProbeResult
            {
                Operation = operation,
                Outcome = GraphQlProbeOutcome.HttpError,
                ErrorCodes = code is not null ? [code] : [],
                Note = ex.Detail,
            };
        }
        catch (OctopusEnergyException ex)
        {
            return new GraphQlAllowListProbeResult
            {
                Operation = operation,
                Outcome = GraphQlProbeOutcome.HttpError,
                Note = ex.Message,
            };
        }
    }

    internal static string? TryReadFirstAccountNumber(JsonElement dataRoot)
    {
        if (!dataRoot.TryGetProperty("viewer", out JsonElement viewer))
        {
            return null;
        }

        if (viewer.TryGetProperty("accounts", out JsonElement accounts)
            && accounts.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement account in accounts.EnumerateArray())
            {
                if (account.TryGetProperty("number", out JsonElement number)
                    && number.ValueKind == JsonValueKind.String
                    && !string.IsNullOrWhiteSpace(number.GetString()))
                {
                    return number.GetString();
                }
            }
        }

        if (!viewer.TryGetProperty("portfolios", out JsonElement portfolios))
        {
            return null;
        }

        foreach (JsonElement portfolio in portfolios.EnumerateArray())
        {
            if (!portfolio.TryGetProperty("accounts", out JsonElement nestedAccounts))
            {
                continue;
            }

            foreach (JsonElement account in nestedAccounts.EnumerateArray())
            {
                if (account.TryGetProperty("number", out JsonElement number)
                    && number.ValueKind == JsonValueKind.String)
                {
                    return number.GetString();
                }
            }
        }

        return null;
    }

    internal static string? TryReadFirstDeviceId(JsonElement dataRoot)
    {
        if (!dataRoot.TryGetProperty("devices", out JsonElement devices)
            || devices.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        foreach (JsonElement device in devices.EnumerateArray())
        {
            if (device.TryGetProperty("id", out JsonElement id)
                && id.ValueKind == JsonValueKind.String
                && !string.IsNullOrWhiteSpace(id.GetString()))
            {
                return id.GetString();
            }
        }

        return null;
    }

    private static string? ExtractErrorCode(string detail)
    {
        Match match = ErrorCodePattern().Match(detail);
        return match.Success ? match.Value : null;
    }

    [GeneratedRegex(@"KT-[A-Z]{2}-\d+", RegexOptions.CultureInvariant)]
    private static partial Regex ErrorCodePattern();

    public async ValueTask DisposeAsync()
    {
        _tokenProvider.Dispose();
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        await Task.CompletedTask;
    }
}
