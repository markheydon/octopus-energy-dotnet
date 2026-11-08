namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class OctoplusQueries
{
    internal const string OctoplusAccountInfo = """
        query OctoplusAccountInfo($accountNumber: String!) {
          octoplusAccountInfo(accountNumber: $accountNumber) {
            enrollmentStatus
            isLoyaltyPointsUser
          }
        }
        """;

    internal const string LoyaltyPointLedgers = """
        query LoyaltyPointLedgers {
          loyaltyPointLedgers {
            id
            ledgerType
            value
            balanceCarriedForward
            postedAt
            accountNumber
          }
        }
        """;

    internal const string LoyaltyPointsBalance = """
        query LoyaltyPointsBalance($accountNumber: String!, $schemeCode: String) {
          loyaltyPointsBalance(input: {
            accountNumber: $accountNumber
            schemeCode: $schemeCode
          }) {
            loyaltyPoints
            totalMonetaryAmount
          }
        }
        """;

    internal const string WheelOfFortuneSpins = """
        query WheelOfFortuneSpins($accountNumber: String!) {
          wheelOfFortuneSpins(accountNumber: $accountNumber) {
            electricity {
              maxSpinsPerMonth
              remainingSpinsThisMonth
              usedSpinsThisMonth
            }
            gas {
              maxSpinsPerMonth
              remainingSpinsThisMonth
              usedSpinsThisMonth
            }
          }
        }
        """;

    internal const string JoinOctoplusCampaign = """
        mutation JoinOctoplusCampaign($accountNumber: String!) {
          joinOctoplusCampaign(accountNumber: $accountNumber) {
            outcome {
              octoplus {
                hasEnrolled
              }
            }
          }
        }
        """;

    internal const string RedeemLoyaltyPointsForAccountCredit = """
        mutation RedeemLoyaltyPointsForAccountCredit(
          $accountNumber: String!
          $points: Int!
          $schemeCode: String
        ) {
          redeemLoyaltyPointsForAccountCredit(input: {
            accountNumber: $accountNumber
            points: $points
            schemeCode: $schemeCode
          }) {
            pointsRedeemed
          }
        }
        """;
}
