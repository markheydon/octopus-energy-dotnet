using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client;

/// <summary>
/// Serialises <see cref="GridSupplyPoint"/> as an underscore-prefixed group id (for example <c>_C</c>).
/// </summary>
internal sealed class GridSupplyPointJsonConverter : JsonConverter<GridSupplyPoint>
{
    /// <inheritdoc />
    public override GridSupplyPoint Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Expected a string for grid supply point group id.");
        }

        string? value = reader.GetString();
        if (!GridSupplyPointParser.TryParse(value, out GridSupplyPoint supplyPoint))
        {
            throw new JsonException($"The value '{value}' is not a valid grid supply point group id.");
        }

        return supplyPoint;
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, GridSupplyPoint value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(GridSupplyPointParser.ToGroupId(value));
    }
}
