using System.Globalization;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal static class RestQuery
{
    internal static string Append(string relativePath, IReadOnlyList<QueryParameter> parameters)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);
        ArgumentNullException.ThrowIfNull(parameters);

        if (parameters.Count == 0)
        {
            return relativePath;
        }

        List<string> pairs = new(parameters.Count);
        foreach (QueryParameter parameter in parameters)
        {
            if (parameter.Value is null)
            {
                continue;
            }

            pairs.Add($"{Uri.EscapeDataString(parameter.Name)}={Uri.EscapeDataString(parameter.Value)}");
        }

        if (pairs.Count == 0)
        {
            return relativePath;
        }

        string separator = relativePath.Contains('?', StringComparison.Ordinal) ? "&" : "?";
        return $"{relativePath}{separator}{string.Join("&", pairs)}";
    }

    internal static string FormatDateTimeOffset(DateTimeOffset value)
    {
        DateTimeOffset normalised = value.ToUniversalTime();
        return normalised.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
    }

    internal readonly record struct QueryParameter(string Name, string? Value);
}
