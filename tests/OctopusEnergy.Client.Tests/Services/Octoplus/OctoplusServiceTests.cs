using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Octoplus;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Octoplus;

public sealed class OctoplusServiceTests
{
    private const string AccountNumber = "A-TEST0001";

    [Fact]
    public async Task GetAccountInfoAsync_WhenAuthenticated_ReturnsInfo()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-octoplus-account-info.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        OctoplusAccountInfo info = await client.Octoplus.GetAccountInfoAsync(AccountNumber, CancellationToken.None);

        Assert.Equal("ENROLLED", info.EnrollmentStatus);
        Assert.True(info.IsLoyaltyPointsUser);
        Assert.Contains("\"accountNumber\":\"A-TEST0001\"", handler.SentRequestBodies[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task ListLoyaltyPointLedgersAsync_WhenAuthenticated_ReturnsLedgers()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-loyalty-point-ledgers.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<LoyaltyPointLedgerEntry> ledgers = await client.Octoplus.ListLoyaltyPointLedgersAsync(
            CancellationToken.None);

        Assert.Single(ledgers);
        Assert.Equal("1250", ledgers[0].BalanceCarriedForward);
    }

    [Fact]
    public async Task GetLoyaltyPointsBalanceAsync_WhenAuthenticated_ReturnsBalance()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-loyalty-points-balance.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        LoyaltyPointsBalance balance = await client.Octoplus.GetLoyaltyPointsBalanceAsync(
            AccountNumber,
            cancellationToken: CancellationToken.None);

        Assert.Equal(1250, balance.Points);
        Assert.Equal(625, balance.TotalMonetaryAmountPence);
    }

    [Fact]
    public async Task GetWheelOfFortuneSpinsAsync_WhenAuthenticated_ReturnsSpins()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-wheel-of-fortune-spins.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        WheelOfFortuneSpinsAvailability spins = await client.Octoplus.GetWheelOfFortuneSpinsAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.Equal(1, spins.Electricity?.RemainingSpinsThisMonth);
        Assert.Equal(0, spins.Gas?.RemainingSpinsThisMonth);
    }

    [Fact]
    public async Task JoinCampaignAsync_WhenAuthenticated_ReturnsOutcome()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-join-octoplus-campaign.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        JoinOctoplusCampaignResult result = await client.Octoplus.JoinCampaignAsync(
            AccountNumber,
            CancellationToken.None);

        Assert.True(result.HasEnrolled);
    }

    [Fact]
    public async Task RedeemPointsForAccountCreditAsync_WhenAuthenticated_ReturnsPointsRedeemed()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-redeem-loyalty-points.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        RedeemLoyaltyPointsResult result = await client.Octoplus.RedeemPointsForAccountCreditAsync(
            AccountNumber,
            points: 50,
            cancellationToken: CancellationToken.None);

        Assert.Equal(50, result.PointsRedeemed);
    }

    [Fact]
    public async Task GetAccountInfoAsync_WhenNoApiKey_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new();

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Octoplus.GetAccountInfoAsync(AccountNumber, CancellationToken.None));
    }

    [Fact]
    public async Task GetLoyaltyPointsBalanceAsync_WhenAccountNumberMissing_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Octoplus.GetLoyaltyPointsBalanceAsync(" ", cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task RedeemPointsForAccountCreditAsync_WhenPointsNotPositive_ThrowsBeforeHttpCall()
    {
        using OctopusEnergyClient client = new("test-api-key");

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Octoplus.RedeemPointsForAccountCreditAsync(AccountNumber, 0, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task GetLoyaltyPointsBalanceAsync_WhenKrakenUnauthorised_ThrowsKrakenException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-allow-list-loyalty-points-denied.json"));

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        await Assert.ThrowsAsync<OctopusEnergyKrakenException>(
            () => client.Octoplus.GetLoyaltyPointsBalanceAsync(AccountNumber, cancellationToken: CancellationToken.None));
    }

    [Fact]
    public async Task ListLoyaltyPointLedgersAsync_WhenEmpty_ReturnsEmptyList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("graphql-obtain-token.json"));
        handler.Enqueue(
            HttpStatusCode.OK,
            """
            {"data":{"loyaltyPointLedgers":[]}}
            """);

        using HttpClient httpClient = new(handler) { BaseAddress = new Uri("https://api.example.test/v1/") };
        using OctopusEnergyClient client = new("test-api-key", httpClient);

        IReadOnlyList<LoyaltyPointLedgerEntry> ledgers = await client.Octoplus.ListLoyaltyPointLedgersAsync(
            CancellationToken.None);

        Assert.Empty(ledgers);
    }
}
