using System.Reflection;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal static class OctopusEnergyUserAgent
{
    internal const string ProductName = "OctopusEnergy.Client";

    private static readonly string ProductValue = CreateValue();

    internal static string Value => ProductValue;

    private static string CreateValue()
    {
        Assembly assembly = typeof(OctopusEnergyClient).Assembly;
        string? informationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (!string.IsNullOrWhiteSpace(informationalVersion))
        {
            int plusIndex = informationalVersion.IndexOf('+', StringComparison.Ordinal);
            if (plusIndex >= 0)
            {
                informationalVersion = informationalVersion[..plusIndex];
            }

            return $"{ProductName}/{informationalVersion}";
        }

        Version? version = assembly.GetName().Version;
        return version is null ? ProductName : $"{ProductName}/{version}";
    }
}
