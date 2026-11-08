using System.Text.Json;

namespace OctopusEnergy.Client.Tests.TestSupport;

internal sealed class GraphQlAllowListMeterContext
{
    internal string? Mpan { get; init; }

    internal string? ElectricitySerialNumber { get; init; }

    internal string? Mprn { get; init; }

    internal string? GasSerialNumber { get; init; }

    internal static GraphQlAllowListMeterContext? TryReadFromAccountMeterPoints(JsonElement data)
    {
        if (!data.TryGetProperty("account", out JsonElement account)
            || !account.TryGetProperty("properties", out JsonElement properties))
        {
            return null;
        }

        string? mpan = null;
        string? electricitySerial = null;
        string? mprn = null;
        string? gasSerial = null;

        foreach (JsonElement property in properties.EnumerateArray())
        {
            if (property.TryGetProperty("electricityMeterPoints", out JsonElement electricityPoints))
            {
                foreach (JsonElement point in electricityPoints.EnumerateArray())
                {
                    if (mpan is null
                        && point.TryGetProperty("mpan", out JsonElement mpanElement)
                        && mpanElement.ValueKind == JsonValueKind.String)
                    {
                        mpan = mpanElement.GetString();
                    }

                    if (electricitySerial is null
                        && point.TryGetProperty("meters", out JsonElement meters))
                    {
                        foreach (JsonElement meter in meters.EnumerateArray())
                        {
                            if (meter.TryGetProperty("serialNumber", out JsonElement serial)
                                && serial.ValueKind == JsonValueKind.String)
                            {
                                electricitySerial = serial.GetString();
                                break;
                            }
                        }
                    }
                }
            }

            if (property.TryGetProperty("gasMeterPoints", out JsonElement gasPoints))
            {
                foreach (JsonElement point in gasPoints.EnumerateArray())
                {
                    if (mprn is null
                        && point.TryGetProperty("mprn", out JsonElement mprnElement)
                        && mprnElement.ValueKind == JsonValueKind.String)
                    {
                        mprn = mprnElement.GetString();
                    }

                    if (gasSerial is null
                        && point.TryGetProperty("meters", out JsonElement meters))
                    {
                        foreach (JsonElement meter in meters.EnumerateArray())
                        {
                            if (meter.TryGetProperty("serialNumber", out JsonElement serial)
                                && serial.ValueKind == JsonValueKind.String)
                            {
                                gasSerial = serial.GetString();
                                break;
                            }
                        }
                    }
                }
            }
        }

        if (mpan is null && mprn is null)
        {
            return null;
        }

        return new GraphQlAllowListMeterContext
        {
            Mpan = mpan,
            ElectricitySerialNumber = electricitySerial,
            Mprn = mprn,
            GasSerialNumber = gasSerial,
        };
    }
}
