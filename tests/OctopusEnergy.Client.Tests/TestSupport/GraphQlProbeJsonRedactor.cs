using System.Text.Json;
using System.Text.RegularExpressions;
using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Tests.TestSupport;

/// <summary>
/// Redacts sensitive values from GraphQL JSON captured during live allow-list probes.
/// </summary>
internal static partial class GraphQlProbeJsonRedactor
{
    private const int MaxLength = 2_048;

    internal static string RedactAndTruncate(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        string redacted = json;
        redacted = KrakenIdPattern().Replace(redacted, "\"id\":\"viewer-id-redacted\"");
        redacted = AccountNumberPattern().Replace(redacted, "A-TEST0001");
        redacted = MpanPattern().Replace(redacted, "1000000000001");
        redacted = MprnPropertyPattern().Replace(redacted, "\"mprn\":\"1234567890\"");
        redacted = SerialNumberPattern().Replace(redacted, "\"serialNumber\":\"TEST-SERIAL-RED\"");
        redacted = JwtPattern().Replace(redacted, "eyJhbGciOiJub25lIn0.redacted.redacted");
        redacted = EmailPattern().Replace(redacted, "user@example.test");
        redacted = UkPostcodePattern().Replace(redacted, "W1 1AA");

        if (redacted.Length <= MaxLength)
        {
            return redacted;
        }

        return redacted[..MaxLength] + "\n…(truncated)";
    }

    internal static string SerializeEnvelope<TData>(GraphQlResponseEnvelope<TData> envelope)
    {
        return JsonSerializer.Serialize(envelope, OctopusTestJsonSerializerOptions.Default);
    }

    [GeneratedRegex(@"""id""\s*:\s*""\d+""", RegexOptions.CultureInvariant)]
    private static partial Regex KrakenIdPattern();

    [GeneratedRegex(@"\bA-[A-Z0-9]+\b", RegexOptions.CultureInvariant)]
    private static partial Regex AccountNumberPattern();

    [GeneratedRegex(@"\b\d{13}\b", RegexOptions.CultureInvariant)]
    private static partial Regex MpanPattern();

    [GeneratedRegex(@"""mprn""\s*:\s*""\d+""", RegexOptions.CultureInvariant)]
    private static partial Regex MprnPropertyPattern();

    [GeneratedRegex(@"""serialNumber"":\s*""[^""]+""", RegexOptions.CultureInvariant)]
    private static partial Regex SerialNumberPattern();

    [GeneratedRegex(@"eyJ[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*\.[A-Za-z0-9_-]*", RegexOptions.CultureInvariant)]
    private static partial Regex JwtPattern();

    [GeneratedRegex(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", RegexOptions.CultureInvariant)]
    private static partial Regex EmailPattern();

    [GeneratedRegex(@"\b[A-Z]{1,2}\d[A-Z\d]?\s?\d[A-Z]{2}\b", RegexOptions.CultureInvariant)]
    private static partial Regex UkPostcodePattern();
}
