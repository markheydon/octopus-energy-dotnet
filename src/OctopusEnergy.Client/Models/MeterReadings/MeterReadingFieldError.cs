using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// A field-level validation error returned inside a successful meter reading mutation payload.
/// </summary>
public sealed class MeterReadingFieldError
{
    /// <summary>
    /// Gets the input field name associated with the error.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Gets the validation messages for the field.
    /// </summary>
    [JsonPropertyName("errors")]
    public IReadOnlyList<string> Errors { get; init; } = [];
}
