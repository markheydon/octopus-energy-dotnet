using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using OctopusEnergy.Client.Infrastructure.Serialization;

namespace OctopusEnergy.Client.Tests.TestSupport;

/// <summary>
/// Test JSON options with a reflection fallback for ad-hoc test models. Production uses source generation only.
/// </summary>
internal static class OctopusTestJsonSerializerOptions
{
    internal static JsonSerializerOptions Default { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        TypeInfoResolver = JsonTypeInfoResolver.Combine(
            OctopusEnergyJsonContext.Default,
            new DefaultJsonTypeInfoResolver()),
    };
}
