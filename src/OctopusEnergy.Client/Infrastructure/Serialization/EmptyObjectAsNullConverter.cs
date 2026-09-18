using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises empty JSON objects as <see langword="null"/> (for example payment methods with no tariff).
/// </summary>
internal sealed class EmptyObjectAsNullConverter<T> : JsonConverter<T?>
    where T : class
{
    /// <inheritdoc />
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected an object for {typeof(T).Name}.");
        }

        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        if (document.RootElement.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (document.RootElement.EnumerateObject().Any())
        {
            return document.RootElement.Deserialize<T>(options);
        }

        return null;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        JsonSerializer.Serialize(writer, value, options);
    }
}
