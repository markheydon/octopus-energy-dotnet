using System.Text.Json;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

internal static class OctopusJsonSerializerOptions
{
    internal static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        return options;
    }
}
