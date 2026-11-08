using System.Globalization;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class CreateElectricityMeterReadingMutationData
{
    [JsonPropertyName("createElectricityMeterReading")]
    public CreateMeterReadingOutputWire? CreateElectricityMeterReading { get; init; }
}

internal sealed class CreateMeterReadingOutputWire
{
    [JsonPropertyName("readingErrors")]
    public IReadOnlyList<MeterReadingFieldErrorWire>? ReadingErrors { get; init; }
}

internal sealed class MeterReadingFieldErrorWire
{
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<string>? Errors { get; init; }
}

internal sealed class CreateElectricityMeterReadingVariables
{
    [JsonPropertyName("mpan")]
    public required string Mpan { get; init; }

    [JsonPropertyName("serialNumber")]
    public required string SerialNumber { get; init; }

    [JsonPropertyName("readAt")]
    public required string ReadAt { get; init; }

    [JsonPropertyName("readings")]
    public required IReadOnlyList<ElectricityReadingInputWire> Readings { get; init; }
}

internal sealed class ElectricityReadingInputWire
{
    [JsonPropertyName("registerId")]
    public string? RegisterId { get; init; }

    [JsonPropertyName("reading")]
    public int Reading { get; init; }
}

internal static class MeterReadingWireFormatting
{
    internal static string FormatReadAtDate(DateTimeOffset readAt)
    {
        DateTimeOffset uk = TimeZoneInfo.ConvertTime(readAt, OctopusEnergyTime.EuropeLondon);
        return uk.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}
