namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class DeviceQueries
{
    internal const string ListDevices = """
        query ListDevices($accountNumber: String!) {
          devices(accountNumber: $accountNumber) {
            id
            name
            provider
            deviceType
            __typename
            status {
              currentState
            }
            ... on SmartFlexVehicle {
              make
              model
            }
            ... on SmartFlexChargePoint {
              make
              model
            }
          }
        }
        """;

    internal const string FlexPlannedDispatches = """
        query FlexPlannedDispatches($deviceId: String!) {
          flexPlannedDispatches(deviceId: $deviceId) {
            start
            end
            type
            energyAddedKwh
          }
        }
        """;
}
