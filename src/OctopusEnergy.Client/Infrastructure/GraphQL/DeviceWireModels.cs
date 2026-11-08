using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal sealed class ListDevicesQueryData
{
    [JsonPropertyName("devices")]
    public IReadOnlyList<DeviceWire>? Devices { get; init; }
}

internal sealed class DeviceWire
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("provider")]
    public string? Provider { get; init; }

    [JsonPropertyName("deviceType")]
    public string? DeviceType { get; init; }

    [JsonPropertyName("__typename")]
    public string? TypeName { get; init; }

    [JsonPropertyName("make")]
    public string? Make { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }

    [JsonPropertyName("status")]
    public DeviceStatusWire? Status { get; init; }
}

internal sealed class DeviceStatusWire
{
    [JsonPropertyName("currentState")]
    public string? CurrentState { get; init; }
}

internal sealed class FlexPlannedDispatchesQueryData
{
    [JsonPropertyName("flexPlannedDispatches")]
    public IReadOnlyList<FlexPlannedDispatchWire>? FlexPlannedDispatches { get; init; }
}

internal sealed class FlexPlannedDispatchWire
{
    [JsonPropertyName("start")]
    public DateTimeOffset Start { get; init; }

    [JsonPropertyName("end")]
    public DateTimeOffset End { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("energyAddedKwh")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public decimal? EnergyAddedKwh { get; init; }
}

internal sealed class ListDevicesQueryVariables
{
    [JsonPropertyName("accountNumber")]
    public required string AccountNumber { get; init; }
}

internal sealed class FlexPlannedDispatchesQueryVariables
{
    [JsonPropertyName("deviceId")]
    public required string DeviceId { get; init; }
}
