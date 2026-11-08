using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.Serialization;

/// <summary>
/// Deserialises API string tokens into <see cref="WireField{TEnum}"/> without throwing on unknown values.
/// </summary>
internal sealed class WireFieldJsonConverter<TEnum> : JsonConverter<WireField<TEnum>>
    where TEnum : struct, Enum
{
    /// <inheritdoc />
    public override WireField<TEnum> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return new WireField<TEnum> { Value = OpenWireEnum.UnknownValue<TEnum>() };
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected a string for {typeof(TEnum).Name} wire field.");
        }

        string? wire = reader.GetString();
        return Parse(wire);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, WireField<TEnum> value, JsonSerializerOptions options)
    {
        string? wire = ToWire(value);
        if (wire is null)
        {
            if (value.UnparsedValue is not null)
            {
                writer.WriteStringValue(value.UnparsedValue);
                return;
            }

            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(wire);
    }

    private static WireField<TEnum> Parse(string? wire) =>
        typeof(TEnum).Name switch
        {
            nameof(ProductEnergyDirection) => (WireField<TEnum>)(object)OpenWireEnum.ParseProductEnergyDirection(wire),
            nameof(MeterRegisterRate) => (WireField<TEnum>)(object)OpenWireEnum.ParseMeterRegisterRate(wire),
            nameof(ProductPaymentMethod) => (WireField<TEnum>)(object)OpenWireEnum.ParseProductPaymentMethod(wire),
            _ => throw new NotSupportedException($"No open wire parser registered for {typeof(TEnum).Name}."),
        };

    private static string? ToWire(WireField<TEnum> value) =>
        typeof(TEnum).Name switch
        {
            nameof(ProductEnergyDirection) => OpenWireEnum.ToWire((ProductEnergyDirection)(object)value.Value!),
            nameof(MeterRegisterRate) => OpenWireEnum.ToWire((MeterRegisterRate)(object)value.Value!),
            nameof(ProductPaymentMethod) => OpenWireEnum.ToWire((ProductPaymentMethod)(object)value.Value!),
            _ => throw new NotSupportedException($"No open wire formatter registered for {typeof(TEnum).Name}."),
        };
}

/// <summary>
/// Deserialises nullable API string tokens into <see cref="WireField{TEnum}"/>.
/// </summary>
internal sealed class NullableWireFieldJsonConverter<TEnum> : JsonConverter<WireField<TEnum>?>
    where TEnum : struct, Enum
{
    private readonly WireFieldJsonConverter<TEnum> _inner = new();

    /// <inheritdoc />
    public override WireField<TEnum>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return _inner.Read(ref reader, typeof(WireField<TEnum>), options);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, WireField<TEnum>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        _inner.Write(writer, value.Value, options);
    }
}
