using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.SmartMeterTelemetry;
namespace OctopusEnergy.Client.Services.SmartMeterTelemetry;

/// <summary>
/// GraphQL Home Mini / CAD telemetry for the authenticated dashboard API key.
/// </summary>
public sealed class SmartMeterTelemetryService : ISmartMeterTelemetryService
{
    private readonly GraphQlClient? _graphQl;

    internal SmartMeterTelemetryService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmartMeterDevice>> ListSmartDevicesAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        ListSmartMeterDevicesQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
        };

        ListSmartMeterDevicesQueryData data = await _graphQl!.ExecuteAsync<ListSmartMeterDevicesQueryData>(
            SmartMeterTelemetryQueries.ListSmartMeterDevices,
            variables,
            cancellationToken).ConfigureAwait(false);

        SmartMeterAccountWire account = data.Account
            ?? throw new OctopusEnergyParseException(
                "The smart meter device query returned no account data for the supplied account number.");

        List<SmartMeterDevice> devices = [];
        CollectElectricityDevices(account, devices);
        CollectGasDevices(account, devices);

        return devices
            .GroupBy(d => d.DeviceId, StringComparer.Ordinal)
            .Select(g => g.First())
            .ToList();
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        return GetTelemetryCoreAsync(deviceId, range: null, cancellationToken);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryAsync(
        string deviceId,
        SmartMeterTelemetryRange range,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(range);
        return GetTelemetryCoreAsync(deviceId, range, cancellationToken);
    }

    private async Task<IReadOnlyList<SmartMeterTelemetryReading>> GetTelemetryCoreAsync(
        string deviceId,
        SmartMeterTelemetryRange? range,
        CancellationToken cancellationToken)
    {
        EnsureGraphQl();
        ValidateDeviceId(deviceId);

        SmartMeterTelemetryQueryVariables variables = new()
        {
            DeviceId = deviceId.Trim(),
            Start = range?.Start,
            End = range?.End,
            Grouping = range is null ? null : ToGraphQlGrouping(range.Grouping),
        };

        SmartMeterTelemetryQueryData data = await _graphQl!.ExecuteAsync<SmartMeterTelemetryQueryData>(
            SmartMeterTelemetryQueries.SmartMeterTelemetry,
            variables,
            cancellationToken).ConfigureAwait(false);

        if (data.SmartMeterTelemetry is not { Count: > 0 })
        {
            return [];
        }

        return data.SmartMeterTelemetry.Select(MapReading).ToList();
    }

    private static void CollectElectricityDevices(SmartMeterAccountWire? account, List<SmartMeterDevice> target)
    {
        if (account?.ElectricityAgreements is not { Count: > 0 })
        {
            return;
        }

        foreach (ElectricityAgreementWire agreement in account.ElectricityAgreements)
        {
            ElectricityMeterPointDiscoveryWire? meterPoint = agreement.MeterPoint;
            if (meterPoint?.Meters is not { Count: > 0 })
            {
                continue;
            }

            foreach (MeterDiscoveryWire meter in meterPoint.Meters)
            {
                AppendDevices(
                    target,
                    meter.SmartDevices,
                    meter.SerialNumber,
                    meterPoint.Mpan,
                    EnergyFuel.Electricity);
            }
        }
    }

    private static void CollectGasDevices(SmartMeterAccountWire? account, List<SmartMeterDevice> target)
    {
        if (account?.GasAgreements is not { Count: > 0 })
        {
            return;
        }

        foreach (GasAgreementWire agreement in account.GasAgreements)
        {
            GasMeterPointDiscoveryWire? meterPoint = agreement.MeterPoint;
            if (meterPoint?.Meters is not { Count: > 0 })
            {
                continue;
            }

            foreach (MeterDiscoveryWire meter in meterPoint.Meters)
            {
                AppendDevices(
                    target,
                    meter.SmartDevices,
                    meter.SerialNumber,
                    meterPoint.Mprn,
                    EnergyFuel.Gas);
            }
        }
    }

    private static void AppendDevices(
        List<SmartMeterDevice> target,
        IReadOnlyList<SmartMeterDeviceWire>? smartDevices,
        string? meterSerialNumber,
        string? supplyPointIdentifier,
        EnergyFuel fuel)
    {
        if (smartDevices is not { Count: > 0 })
        {
            return;
        }

        foreach (SmartMeterDeviceWire wire in smartDevices)
        {
            if (string.IsNullOrWhiteSpace(wire.DeviceId))
            {
                continue;
            }

            target.Add(new SmartMeterDevice
            {
                DeviceId = wire.DeviceId,
                DeviceType = wire.Type,
                SerialNumber = wire.SerialNumber,
                MeterSerialNumber = meterSerialNumber,
                SupplyPointIdentifier = supplyPointIdentifier,
                Fuel = fuel,
            });
        }
    }

    private static SmartMeterTelemetryReading MapReading(SmartMeterTelemetryReadingWire wire)
    {
        return new SmartMeterTelemetryReading
        {
            ReadAt = wire.ReadAt,
            Consumption = wire.Consumption,
            ConsumptionDelta = wire.ConsumptionDelta,
            CostDelta = wire.CostDelta,
            CostDeltaWithTax = wire.CostDeltaWithTax,
            Demand = wire.Demand,
            Export = wire.Export,
        };
    }

    private static string ToGraphQlGrouping(SmartMeterTelemetryGrouping grouping) =>
        grouping switch
        {
            SmartMeterTelemetryGrouping.TenSeconds => "TEN_SECONDS",
            SmartMeterTelemetryGrouping.OneMinute => "ONE_MINUTE",
            SmartMeterTelemetryGrouping.FiveMinutes => "FIVE_MINUTES",
            SmartMeterTelemetryGrouping.HalfHourly => "HALF_HOURLY",
            SmartMeterTelemetryGrouping.Hourly => "HOURLY",
            _ => throw new OctopusEnergyRequestException($"Unsupported telemetry grouping: {grouping}."),
        };

    private void EnsureGraphQl()
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query smart meter telemetry via GraphQL.");
        }
    }

    private static void ValidateAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new OctopusEnergyRequestException("Account number is required.");
        }
    }

    private static void ValidateDeviceId(string deviceId)
    {
        if (string.IsNullOrWhiteSpace(deviceId))
        {
            throw new OctopusEnergyRequestException("Device id is required.");
        }
    }
}
