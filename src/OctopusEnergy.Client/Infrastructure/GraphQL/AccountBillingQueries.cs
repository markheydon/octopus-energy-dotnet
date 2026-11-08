namespace OctopusEnergy.Client.Infrastructure.GraphQL;

internal static class AccountBillingQueries
{
    internal const string ListBills = """
        query AccountBills(
          $accountNumber: String!
          $first: Int!
          $after: String
          $includeBillsWithoutPDF: Boolean!
        ) {
          account(accountNumber: $accountNumber) {
            bills(
              first: $first
              after: $after
              includeBillsWithoutPDF: $includeBillsWithoutPDF
            ) {
              edges {
                node {
                  id
                  billType
                  issuedDate
                  temporaryUrl
                }
              }
              pageInfo {
                hasNextPage
                hasPreviousPage
                endCursor
                startCursor
              }
            }
          }
        }
        """;

    internal const string ListTransactions = """
        query AccountTransactions(
          $accountNumber: String!
          $first: Int!
          $after: String
          $fromDate: Date
          $toDate: Date
        ) {
          account(accountNumber: $accountNumber) {
            transactions(
              first: $first
              after: $after
              fromDate: $fromDate
              toDate: $toDate
            ) {
              edges {
                node {
                  id
                  postedDate
                  __typename
                  amounts {
                    net
                    tax
                    gross
                  }
                }
              }
              pageInfo {
                hasNextPage
                hasPreviousPage
                endCursor
                startCursor
              }
            }
          }
        }
        """;

    internal const string ListPayments = """
        query AccountPayments(
          $accountNumber: String!
          $first: Int!
          $after: String
        ) {
          account(accountNumber: $accountNumber) {
            payments(first: $first, after: $after) {
              edges {
                node {
                  id
                  amount
                  paymentDate
                }
              }
              pageInfo {
                hasNextPage
                hasPreviousPage
                endCursor
                startCursor
              }
            }
          }
        }
        """;
}
