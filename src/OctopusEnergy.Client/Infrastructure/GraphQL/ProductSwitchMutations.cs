namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class ProductSwitchMutations
{
    internal const string Initiate = """
        mutation InitiateProductSwitch(
          $accountNumber: String!
          $switchDate: Date!
          $quotedProductId: ID
          $offerIdentifier: ID
          $suppressCommunications: Boolean
        ) {
          initiateProductSwitch(input: {
            accountNumber: $accountNumber
            switchDate: $switchDate
            quotedProductId: $quotedProductId
            offerIdentifier: $offerIdentifier
            suppressCommunications: $suppressCommunications
          }) {
            productCode
            switchDate
          }
        }
        """;
}
