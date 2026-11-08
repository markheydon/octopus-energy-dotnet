using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Models.MeterReadings;

/// <summary>
/// The result of <c>createElectricityMeterReading</c> when the GraphQL call succeeds.
/// </summary>
public sealed class ElectricityMeterReadingResult
{
    /// <summary>
    /// Gets field-level validation errors. An empty list means the mutation payload reported no field errors.
    /// </summary>
    [JsonPropertyName("readingErrors")]
    public IReadOnlyList<MeterReadingFieldError> ReadingErrors { get; init; } = [];
}
