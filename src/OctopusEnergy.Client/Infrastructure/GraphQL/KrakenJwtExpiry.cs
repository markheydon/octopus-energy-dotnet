using System.Text.Json;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class KrakenJwtExpiry
{
    internal static DateTimeOffset? TryReadExpiryUtc(string jwt)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jwt);

        ReadOnlySpan<char> span = jwt.AsSpan();
        int firstDot = span.IndexOf('.');
        if (firstDot < 0)
        {
            return null;
        }

        ReadOnlySpan<char> remainder = span[(firstDot + 1)..];
        int secondDot = remainder.IndexOf('.');
        if (secondDot < 0)
        {
            return null;
        }

        string payloadSegment = remainder[..secondDot].ToString();

        try
        {
            byte[] payloadBytes = Base64UrlDecode(payloadSegment);
            using JsonDocument document = JsonDocument.Parse(payloadBytes);

            if (document.RootElement.TryGetProperty("exp", out JsonElement expElement) &&
                expElement.TryGetInt64(out long unixSeconds))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixSeconds);
            }
        }
        catch (JsonException)
        {
        }
        catch (FormatException)
        {
        }

        return null;
    }

    private static byte[] Base64UrlDecode(string segment)
    {
        string padded = segment.Replace('-', '+').Replace('_', '/');
        switch (padded.Length % 4)
        {
            case 2:
                padded += "==";
                break;
            case 3:
                padded += "=";
                break;
        }

        return Convert.FromBase64String(padded);
    }
}
