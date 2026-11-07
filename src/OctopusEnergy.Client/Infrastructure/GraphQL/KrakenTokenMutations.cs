namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class KrakenTokenMutations
{
    internal const string ObtainKrakenToken = """
        mutation ObtainKrakenToken($input: ObtainJSONWebTokenInput!) {
          obtainKrakenToken(input: $input) {
            token
            refreshToken
            refreshExpiresIn
          }
        }
        """;
}
