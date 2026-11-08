namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class MeterReadingQueries
{
    internal const string ReadingConsentGranularity = """
        query ReadingConsentGranularity($mpan: String!) {
          readingConsentGranularity(mpan: $mpan) {
            granularityValue
          }
        }
        """;
}
