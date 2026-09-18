using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises JSON objects keyed by GSP group ids (<c>_A</c> … <c>_P</c>) as
/// <see cref="IReadOnlyDictionary{GridSupplyPoint, TValue}"/>.
/// </summary>
/// <remarks>
/// JSON <c>null</c> and absent properties both yield an empty dictionary. Unrecognised GSP keys
/// are skipped so product detail can still be read when the API adds regions before the SDK enum
/// is updated.
/// </remarks>
internal sealed class GridSupplyPointDictionaryConverter<TValue> : JsonConverter<IReadOnlyDictionary<GridSupplyPoint, TValue>>
{
    /// <inheritdoc />
    public override IReadOnlyDictionary<GridSupplyPoint, TValue> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new Dictionary<GridSupplyPoint, TValue>();
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected an object keyed by grid supply point group ids.");
        }

        Dictionary<GridSupplyPoint, TValue> dictionary = new();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return dictionary;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Expected a property name for a grid supply point key.");
            }

            string? propertyName = reader.GetString();
            reader.Read();

            if (!GridSupplyPointParser.TryParse(propertyName, out GridSupplyPoint supplyPoint))
            {
                JsonSerializer.Deserialize<TValue>(ref reader, options);
                continue;
            }
            TValue? value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            if (value is not null)
            {
                dictionary[supplyPoint] = value;
            }
        }

        throw new JsonException("Unexpected end of JSON while reading a grid supply point dictionary.");
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        IReadOnlyDictionary<GridSupplyPoint, TValue> value,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (KeyValuePair<GridSupplyPoint, TValue> entry in value)
        {
            writer.WritePropertyName(GridSupplyPointParser.ToGroupId(entry.Key));
            JsonSerializer.Serialize(writer, entry.Value, options);
        }

        writer.WriteEndObject();
    }
}
