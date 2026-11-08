using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Live;

/// <summary>
/// Opt-in live probes that record the customer GraphQL allow-list against the UK API.
/// </summary>
public sealed class LiveGraphQlAllowListProbeTests(ITestOutputHelper output)
{
    private const string AccountNumberEnvironmentVariable = "OCTOPUS_ENERGY_ACCOUNT_NUMBER";

    private static readonly TimeSpan TelemetryDelay = TimeSpan.FromSeconds(2);

    [Fact]
    public async Task RunAllowListProbes_WhenLiveEnabled_WritesSummary()
    {
        LiveTestGate.SkipUnlessEnabled();

        string? apiKey = Environment.GetEnvironmentVariable(LiveTestGate.ApiKeyEnvironmentVariable);
        Assert.False(string.IsNullOrWhiteSpace(apiKey), $"Set {LiveTestGate.ApiKeyEnvironmentVariable}.");

        await using GraphQlAllowListProbe probe = GraphQlAllowListProbe.Create(apiKey!);

        List<GraphQlAllowListProbeResult> results = [];

        if (!await probe.TryEnsureTokenAsync(CancellationToken.None))
        {
            results.Add(new GraphQlAllowListProbeResult
            {
                Operation = "obtainKrakenToken",
                Outcome = GraphQlProbeOutcome.TokenObtainFailed,
                ErrorCodes = ["KT-CT-11334"],
                Note = "Dashboard API access is not enabled for this account user.",
            });

            WriteSummary(results);
            return;
        }

        GraphQlAllowListProbeResult viewerResult = await probe.ProbeAsync(
            "viewer",
            AllowListProbeQueries.Viewer,
            variables: null,
            CancellationToken.None);
        results.Add(viewerResult);

        string? accountNumber = ResolveAccountNumber(viewerResult);
        GraphQlAllowListMeterContext? meters = null;

        if (accountNumber is not null)
        {
            results.Add(await probe.ProbeAsync(
                "account",
                AllowListProbeQueries.Account,
                new { accountNumber },
                CancellationToken.None));

            GraphQlAllowListProbeResult meterPointsResult = await probe.ProbeAsync(
                "accountMeterPoints",
                AllowListProbeQueries.AccountMeterPoints,
                new { accountNumber },
                CancellationToken.None);
            results.Add(meterPointsResult);
            meters = meterPointsResult.DiscoveredMeterContext;

            GraphQlAllowListProbeResult devicesResult = await probe.ProbeAsync(
                "devices",
                AllowListProbeQueries.Devices,
                new { accountNumber },
                CancellationToken.None);
            results.Add(devicesResult);

            if (devicesResult.Outcome == GraphQlProbeOutcome.Success
                && !string.IsNullOrWhiteSpace(devicesResult.DiscoveredDeviceId))
            {
                results.Add(await probe.ProbeAsync(
                    "flexPlannedDispatches",
                    AllowListProbeQueries.FlexPlannedDispatchesForDevice,
                    new { deviceId = devicesResult.DiscoveredDeviceId },
                    CancellationToken.None,
                    note: "Uses first device id from devices probe."));
            }
            else
            {
                results.Add(await probe.ProbeAsync(
                    "flexPlannedDispatches",
                    AllowListProbeQueries.FlexPlannedDispatches,
                    variables: null,
                    CancellationToken.None,
                    note: "No device id from devices; placeholder device id."));
            }

            results.Add(await probe.ProbeAsync(
                "accountBills",
                AllowListProbeQueries.AccountBills,
                new { accountNumber },
                CancellationToken.None));

            results.Add(await probe.ProbeAsync(
                "accountTransactions",
                AllowListProbeQueries.AccountTransactions,
                new { accountNumber },
                CancellationToken.None));

            results.Add(await probe.ProbeAsync(
                "accountPayments",
                AllowListProbeQueries.AccountPayments,
                new { accountNumber },
                CancellationToken.None));

            results.Add(await probe.ProbeAsync(
                "initiateProductSwitch",
                AllowListProbeQueries.InitiateProductSwitch,
                new { accountNumber },
                CancellationToken.None,
                note: "Uses a nil quoted product id to avoid switching."));
        }
        else
        {
            output.WriteLine("No account number from viewer; set OCTOPUS_ENERGY_ACCOUNT_NUMBER for account-scoped probes.");
        }

        results.Add(await probe.ProbeAsync(
            "rateLimitInfo",
            AllowListProbeQueries.RateLimitInfo,
            variables: null,
            CancellationToken.None));

        if (!string.IsNullOrWhiteSpace(meters?.Mpan))
        {
            results.Add(await probe.ProbeAsync(
                "readingConsentGranularity",
                AllowListProbeQueries.ReadingConsentGranularity,
                new { mpan = meters.Mpan },
                CancellationToken.None));
        }
        else
        {
            results.Add(new GraphQlAllowListProbeResult
            {
                Operation = "readingConsentGranularity",
                Outcome = GraphQlProbeOutcome.Skipped,
                Note = "Skipped: no import MPAN discovered from accountMeterPoints.",
            });
        }

        if (accountNumber is null)
        {
            results.Add(await probe.ProbeAsync(
                "flexPlannedDispatches",
                AllowListProbeQueries.FlexPlannedDispatches,
                variables: null,
                CancellationToken.None,
                note: "Placeholder device id."));
        }

        await Task.Delay(TelemetryDelay, CancellationToken.None);

        results.Add(await probe.ProbeAsync(
            "smartMeterTelemetry",
            AllowListProbeQueries.SmartMeterTelemetry,
            variables: null,
            CancellationToken.None,
            note: "Placeholder device id."));

        results.Add(await probe.ProbeAsync(
            "loyaltyPointsBalance",
            AllowListProbeQueries.LoyaltyPointsBalance,
            variables: null,
            CancellationToken.None,
            note: "Octoplus / loyalty proxy field on Query root."));

        if (!string.IsNullOrWhiteSpace(meters?.Mpan)
            && !string.IsNullOrWhiteSpace(meters.ElectricitySerialNumber))
        {
            results.Add(await probe.ProbeAsync(
                "createElectricityMeterReading",
                AllowListProbeQueries.CreateElectricityMeterReading,
                new { mpan = meters.Mpan, serialNumber = meters.ElectricitySerialNumber },
                CancellationToken.None,
                note: "Historic readAt date; expects validation or permission error, not a persisted reading."));
        }
        else
        {
            results.Add(new GraphQlAllowListProbeResult
            {
                Operation = "createElectricityMeterReading",
                Outcome = GraphQlProbeOutcome.Skipped,
                Note = "Skipped: no MPAN or electricity serial discovered.",
            });
        }

        if (!string.IsNullOrWhiteSpace(meters?.Mprn) && !string.IsNullOrWhiteSpace(meters.GasSerialNumber))
        {
            results.Add(await probe.ProbeAsync(
                "createGasMeterReading",
                AllowListProbeQueries.CreateGasMeterReading,
                new { mprn = meters.Mprn, serialNumber = meters.GasSerialNumber },
                CancellationToken.None,
                note: "Historic readAt date; expects validation or permission error."));
        }
        else if (accountNumber is not null)
        {
            results.Add(await probe.ProbeAsync(
                "createGasMeterReading",
                AllowListProbeQueries.CreateGasMeterReading,
                new { mprn = "1234567890123", serialNumber = "FAKE-GAS-SERIAL" },
                CancellationToken.None,
                note: "Fake MPRN when no gas meter on account; documents unauthorised path for matrix regeneration."));
        }

        results.Add(await probe.ProbeAsync(
            "partnerViewer",
            AllowListProbeQueries.PartnerViewer,
            variables: null,
            CancellationToken.None));

        results.Add(await probe.ProbeAsync(
            "leads",
            AllowListProbeQueries.PartnerLeads,
            variables: null,
            CancellationToken.None));

        WriteSummary(results);
    }

    private static string? ResolveAccountNumber(GraphQlAllowListProbeResult viewerResult)
    {
        string? fromEnvironment = Environment.GetEnvironmentVariable(AccountNumberEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment.Trim();
        }

        return string.IsNullOrWhiteSpace(viewerResult.DiscoveredAccountNumber)
            ? null
            : viewerResult.DiscoveredAccountNumber;
    }

    private void WriteSummary(IReadOnlyList<GraphQlAllowListProbeResult> results)
    {
        output.WriteLine("## GraphQL customer allow-list probe summary");
        output.WriteLine(string.Empty);
        output.WriteLine("| Operation | Outcome | Error codes |");
        output.WriteLine("|---|---|---|");

        foreach (GraphQlAllowListProbeResult result in results)
        {
            string codes = result.ErrorCodes.Count > 0
                ? string.Join(", ", result.ErrorCodes)
                : "-";
            output.WriteLine($"| {result.Operation} | {result.Outcome} | {codes} |");
        }

        output.WriteLine(string.Empty);
        foreach (GraphQlAllowListProbeResult result in results)
        {
            output.WriteLine($"### {result.Operation}");
            if (result.Note is not null)
            {
                output.WriteLine($"Note: {result.Note}");
            }

            if (result.RedactedJson is not null)
            {
                output.WriteLine("```json");
                output.WriteLine(result.RedactedJson);
                output.WriteLine("```");
            }

            output.WriteLine(string.Empty);
        }
    }
}
