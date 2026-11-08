namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class ViewerQueries
{
    internal const string GetViewer = """
        query Viewer {
          viewer {
            id
            accounts {
              number
            }
            portfolios {
              accounts {
                number
              }
            }
          }
        }
        """;
}
