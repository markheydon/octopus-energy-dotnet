using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

internal static class OctopusJsonSerializerOptions
{
    internal static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = JsonTypeInfoResolver.Combine(
                OctopusEnergyJsonContext.Default,
                new DefaultJsonTypeInfoResolver()),
        };

        return options;
    }
}
