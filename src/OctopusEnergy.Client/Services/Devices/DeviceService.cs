using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.Devices;

namespace OctopusEnergy.Client.Services.Devices;

/// <summary>
/// GraphQL SmartFlex device lists for the authenticated dashboard API key.
/// </summary>
public sealed class DeviceService : IDeviceService
{
    private readonly GraphQlClient? _graphQl;

    internal DeviceService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmartFlexDevice>> ListDevicesAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        ListDevicesQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
        };

        ListDevicesQueryData data = await _graphQl!.ExecuteAsync<ListDevicesQueryData>(
            DeviceQueries.ListDevices,
            variables,
            cancellationToken).ConfigureAwait(false);

        if (data.Devices is not { Count: > 0 })
        {
            return [];
        }

        return data.Devices.Select(MapDevice).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FlexPlannedDispatch>> ListFlexPlannedDispatchesAsync(
        string deviceId,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateDeviceId(deviceId);

        FlexPlannedDispatchesQueryVariables variables = new()
        {
            DeviceId = deviceId.Trim(),
        };

        FlexPlannedDispatchesQueryData data = await _graphQl!.ExecuteAsync<FlexPlannedDispatchesQueryData>(
            DeviceQueries.FlexPlannedDispatches,
            variables,
            cancellationToken).ConfigureAwait(false);

        if (data.FlexPlannedDispatches is not { Count: > 0 })
        {
            return [];
        }

        return data.FlexPlannedDispatches
            .Select(MapDispatch)
            .OrderBy(d => d.Start)
            .ThenBy(d => d.End)
            .ToList();
    }

    private static SmartFlexDevice MapDevice(DeviceWire wire)
    {
        return new SmartFlexDevice
        {
            Id = wire.Id,
            Name = wire.Name,
            DeviceType = wire.DeviceType,
            Provider = wire.Provider,
            TypeName = wire.TypeName,
            Make = wire.Make,
            Model = wire.Model,
            CurrentState = wire.Status?.CurrentState,
        };
    }

    private static FlexPlannedDispatch MapDispatch(FlexPlannedDispatchWire wire)
    {
        return new FlexPlannedDispatch
        {
            Start = wire.Start,
            End = wire.End,
            Type = wire.Type,
            EnergyAddedKwh = wire.EnergyAddedKwh,
        };
    }

    private void EnsureGraphQl()
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query SmartFlex devices via GraphQL.");
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
