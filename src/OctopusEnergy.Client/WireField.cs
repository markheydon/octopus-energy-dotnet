namespace OctopusEnergy.Client;

/// <summary>
/// A deserialised API enum wire field with optional preservation of unknown tokens.
/// </summary>
/// <typeparam name="TEnum">Open wire enum type with an <c>Unknown</c> member.</typeparam>
public readonly struct WireField<TEnum> : IEquatable<WireField<TEnum>>
    where TEnum : struct, Enum
{
    /// <summary>
    /// Parsed enum value.
    /// </summary>
    public TEnum Value { get; init; }

    /// <summary>
    /// Original API token when <see cref="Value"/> is the enum's <c>Unknown</c> member; otherwise <see langword="null"/>.
    /// </summary>
    public string? UnparsedValue { get; init; }

    /// <summary>
    /// Whether <see cref="Value"/> is a known Octopus token (not <c>Unknown</c>).
    /// </summary>
    public bool IsKnown => !IsUnknownValue(Value);

    private static bool IsUnknownValue(TEnum value)
    {
        string? name = Enum.GetName(value);
        return string.Equals(name, "Unknown", StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public bool Equals(WireField<TEnum> other) =>
        EqualityComparer<TEnum>.Default.Equals(Value, other.Value)
        && string.Equals(UnparsedValue, other.UnparsedValue, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is WireField<TEnum> other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Value, UnparsedValue);

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(WireField<TEnum> left, WireField<TEnum> right) => left.Equals(right);

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(WireField<TEnum> left, WireField<TEnum> right) => !left.Equals(right);
}
