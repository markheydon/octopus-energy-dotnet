using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

internal static class OctopusJsonSerializerOptions
{
    internal static JsonSerializerOptions Default { get; } = CreateDefault();

    private static JsonSerializerOptions CreateDefault()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            TypeInfoResolver = OctopusEnergyJsonContext.Default,
        };
    }

    internal static async Task<T> DeserializeAsync<T>(Stream utf8Json, CancellationToken cancellationToken)
    {
        JsonTypeInfo typeInfo = OctopusEnergyJsonContext.Default.GetTypeInfo(typeof(T))
            ?? throw new OctopusEnergyParseException(
                $"The type {typeof(T).FullName} is not registered for JSON source generation.");

        object? value = await JsonSerializer.DeserializeAsync(utf8Json, typeInfo, cancellationToken).ConfigureAwait(false);

        return value is T typed
            ? typed
            : throw new OctopusEnergyParseException("The API returned JSON null where a response model was expected.");
    }
}
