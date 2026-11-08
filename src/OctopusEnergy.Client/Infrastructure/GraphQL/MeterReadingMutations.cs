namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class MeterReadingMutations
{
    internal const string CreateElectricity = """
        mutation CreateElectricityMeterReading(
          $mpan: String!
          $serialNumber: String!
          $readAt: Date!
          $readings: [ReadingInputType!]!
        ) {
          createElectricityMeterReading(
            mpan: $mpan
            serialNumber: $serialNumber
            readAt: $readAt
            readings: $readings
          ) {
            readingErrors {
              field
              errors
            }
          }
        }
        """;
}
