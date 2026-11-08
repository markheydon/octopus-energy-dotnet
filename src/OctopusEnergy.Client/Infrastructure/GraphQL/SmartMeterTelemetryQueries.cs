namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class SmartMeterTelemetryQueries
{
    internal const string ListSmartMeterDevices = """
        query ListSmartMeterDevices($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            electricityAgreements {
              meterPoint {
                mpan
                meters {
                  serialNumber
                  smartDevices {
                    deviceId
                    type
                    serialNumber
                  }
                }
              }
            }
            gasAgreements {
              meterPoint {
                mprn
                meters {
                  serialNumber
                  smartDevices {
                    deviceId
                    type
                    serialNumber
                  }
                }
              }
            }
          }
        }
        """;

    internal const string SmartMeterTelemetry = """
        query SmartMeterTelemetry(
          $deviceId: String!
          $start: DateTime
          $end: DateTime
          $grouping: TelemetryGrouping
        ) {
          smartMeterTelemetry(
            deviceId: $deviceId
            start: $start
            end: $end
            grouping: $grouping
          ) {
            readAt
            consumption
            consumptionDelta
            costDelta
            costDeltaWithTax
            demand
            export
          }
        }
        """;
}
