namespace OctopusEnergy.Client.Tests.Live;

internal static class AllowListProbeQueries
{
    internal const string Viewer = """
        query ViewerAllowListProbe {
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

    internal const string Account = """
        query AccountAllowListProbe($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            number
            __typename
          }
        }
        """;

    internal const string AccountMeterPoints = """
        query AccountMeterPointsAllowListProbe($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            properties {
              electricityMeterPoints {
                mpan
                meters {
                  serialNumber
                }
              }
              gasMeterPoints {
                mprn
                meters {
                  serialNumber
                }
              }
            }
          }
        }
        """;

    internal const string RateLimitInfo = """
        query RateLimitInfoAllowListProbe {
          rateLimitInfo {
            __typename
          }
        }
        """;

    internal const string ReadingConsentGranularity = """
        query ReadingConsentAllowListProbe($mpan: String!) {
          readingConsentGranularity(mpan: $mpan) {
            __typename
          }
        }
        """;

    internal const string Devices = """
        query DevicesAllowListProbe($accountNumber: String!) {
          devices(accountNumber: $accountNumber) {
            __typename
          }
        }
        """;

    internal const string AccountBills = """
        query AccountBillsAllowListProbe($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            bills(first: 1, includeBillsWithoutPDF: true) {
              edges {
                node {
                  id
                  billType
                  issuedDate
                  temporaryUrl
                }
              }
            }
          }
        }
        """;

    internal const string AccountTransactions = """
        query AccountTransactionsAllowListProbe($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            transactions(first: 1) {
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
            }
          }
        }
        """;

    internal const string AccountPayments = """
        query AccountPaymentsAllowListProbe($accountNumber: String!) {
          account(accountNumber: $accountNumber) {
            payments(first: 1) {
              edges {
                node {
                  id
                  amount
                  paymentDate
                }
              }
            }
          }
        }
        """;

    internal const string FlexPlannedDispatches = """
        query FlexPlannedDispatchesAllowListProbe {
          flexPlannedDispatches(deviceId: "0000000000000000") {
            __typename
          }
        }
        """;

    internal const string SmartMeterTelemetry = """
        query SmartMeterTelemetryAllowListProbe {
          smartMeterTelemetry(deviceId: "0000000000000000") {
            __typename
          }
        }
        """;

    internal const string LoyaltyPointsBalance = """
        query LoyaltyPointsAllowListProbe {
          loyaltyPointsBalance {
            __typename
          }
        }
        """;

    internal const string CreateElectricityMeterReading = """
        mutation CreateElectricityReadingAllowListProbe(
          $mpan: String!
          $serialNumber: String!
        ) {
          createElectricityMeterReading(
            mpan: $mpan
            serialNumber: $serialNumber
            readings: [{ reading: 1 }]
            readAt: "2020-01-01"
          ) {
            __typename
          }
        }
        """;

    internal const string CreateGasMeterReading = """
        mutation CreateGasReadingAllowListProbe(
          $mprn: String!
          $serialNumber: String!
        ) {
          createGasMeterReading(
            mprn: $mprn
            serialNumber: $serialNumber
            reading: 1
            readAt: "2020-01-01"
          ) {
            __typename
          }
        }
        """;

    internal const string InitiateProductSwitch = """
        mutation InitiateProductSwitchAllowListProbe($accountNumber: String!) {
          initiateProductSwitch(
            input: {
              accountNumber: $accountNumber
              quotedProductId: "00000000-0000-0000-0000-000000000000"
              switchDate: "2099-01-01"
            }
          ) {
            __typename
          }
        }
        """;

    internal const string PartnerViewer = """
        query PartnerViewerAllowListProbe {
          partnerViewer {
            __typename
          }
        }
        """;

    internal const string PartnerLeads = """
        query PartnerLeadsAllowListProbe {
          leads(first: 1) {
            edges {
              node {
                __typename
              }
            }
          }
        }
        """;
}
