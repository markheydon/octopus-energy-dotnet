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

    internal const string CreateGas = """
        mutation CreateGasMeterReading(
          $mprn: String!
          $serialNumber: String!
          $readAt: Date!
          $reading: Int!
        ) {
          createGasMeterReading(
            mprn: $mprn
            serialNumber: $serialNumber
            readAt: $readAt
            reading: $reading
          ) {
            readingErrors {
              field
              errors
            }
          }
        }
        """;
}
