using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class ListSmartMeterDevicesQueryData
{
    [JsonPropertyName("account")]
    public SmartMeterAccountWire? Account { get; init; }
}

internal sealed class SmartMeterAccountWire
{
    [JsonPropertyName("electricityAgreements")]
    public IReadOnlyList<ElectricityAgreementWire>? ElectricityAgreements { get; init; }

    [JsonPropertyName("gasAgreements")]
    public IReadOnlyList<GasAgreementWire>? GasAgreements { get; init; }
}

internal sealed class ElectricityAgreementWire
{
    [JsonPropertyName("meterPoint")]
    public ElectricityMeterPointDiscoveryWire? MeterPoint { get; init; }
}

internal sealed class GasAgreementWire
{
    [JsonPropertyName("meterPoint")]
    public GasMeterPointDiscoveryWire? MeterPoint { get; init; }
}

internal sealed class ElectricityMeterPointDiscoveryWire
{
    [JsonPropertyName("mpan")]
    public string? Mpan { get; init; }

    [JsonPropertyName("meters")]
    public IReadOnlyList<MeterDiscoveryWire>? Meters { get; init; }
}

internal sealed class GasMeterPointDiscoveryWire
{
    [JsonPropertyName("mprn")]
    public string? Mprn { get; init; }

    [JsonPropertyName("meters")]
    public IReadOnlyList<MeterDiscoveryWire>? Meters { get; init; }
}

internal sealed class MeterDiscoveryWire
{
    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; init; }

    [JsonPropertyName("smartDevices")]
    public IReadOnlyList<SmartMeterDeviceWire>? SmartDevices { get; init; }
}

internal sealed class SmartMeterDeviceWire
{
    [JsonPropertyName("deviceId")]
    public string? DeviceId { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("serialNumber")]
    public string? SerialNumber { get; init; }
}

internal sealed class ListSmartMeterDevicesQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }
}

internal sealed class SmartMeterTelemetryQueryData
{
    [JsonPropertyName("smartMeterTelemetry")]
    public IReadOnlyList<SmartMeterTelemetryReadingWire>? SmartMeterTelemetry { get; init; }
}

internal sealed class SmartMeterTelemetryReadingWire
{
    [JsonPropertyName("readAt")]
    public DateTimeOffset? ReadAt { get; init; }

    [JsonPropertyName("consumption")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Consumption { get; init; }

    [JsonPropertyName("consumptionDelta")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? ConsumptionDelta { get; init; }

    [JsonPropertyName("costDelta")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CostDelta { get; init; }

    [JsonPropertyName("costDeltaWithTax")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? CostDeltaWithTax { get; init; }

    [JsonPropertyName("demand")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Demand { get; init; }

    [JsonPropertyName("export")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? Export { get; init; }
}

internal sealed class SmartMeterTelemetryQueryVariables
{
    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; init; }

    [JsonPropertyName("start")]
    public DateTimeOffset? Start { get; init; }

    [JsonPropertyName("end")]
    public DateTimeOffset? End { get; init; }

    [JsonPropertyName("grouping")]
    public string? Grouping { get; init; }
}
