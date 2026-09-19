using System.Net;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Tests.TestSupport;

namespace OctopusEnergy.Client.Tests.Services.Accounts;

public sealed class AccountServiceTests
{
    [Fact]
    public async Task GetAsync_WhenValidAccountNumber_ReturnsAccountDetail()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("accounts-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        Account account = await client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None);

        Assert.Equal("A-TEST0001", account.Number);
        Assert.Single(account.Properties);

        AccountProperty property = account.Properties[0];
        Assert.Equal(9000001, property.Id);
        Assert.Equal("1 Example Street", property.AddressLine1);
        Assert.Equal("W1 1AA", property.Postcode);
        Assert.Equal(3, property.ElectricityMeterPoints.Count);
        Assert.Single(property.GasMeterPoints);

        ElectricityMeterPoint importPoint = property.ElectricityMeterPoints[0];
        Assert.Equal("1000000000001", importPoint.Mpan);
        Assert.Equal(2, importPoint.Meters.Count);
        Assert.Equal("1111111111", importPoint.Meters[0].SerialNumber);
        Assert.Equal("2222222222", importPoint.Meters[1].SerialNumber);
        Assert.Equal("STANDARD", importPoint.Meters[0].Registers[0].Rate);
        Assert.Equal(2, importPoint.Agreements.Count);
        Assert.Null(importPoint.Agreements[1].ValidTo);
        Assert.Equal(new DateTimeOffset(2023, 4, 1, 0, 0, 0, TimeSpan.FromHours(1)), importPoint.Agreements[1].ValidFrom);
        TariffCode? activeAgreementTariff = importPoint.Agreements[1].ParsedTariffCode;
        Assert.NotNull(activeAgreementTariff);
        Assert.Equal("E-1R-VAR-22-11-01-N", activeAgreementTariff.Value.ToString());
        Assert.False(importPoint.IsExport);

        ElectricityMeterPoint exportPoint = property.ElectricityMeterPoints[1];
        Assert.Equal("1000000000002", exportPoint.Mpan);
        Assert.True(exportPoint.IsExport);

        ElectricityMeterPoint missingExportFlag = property.ElectricityMeterPoints[2];
        Assert.Null(missingExportFlag.IsExport);

        GasMeterPoint gasPoint = property.GasMeterPoints[0];
        Assert.Equal("1234567890", gasPoint.Mprn);
        Assert.Equal(2, gasPoint.Meters.Count);
        Assert.Null(gasPoint.Agreements[0].ValidTo);

        Assert.Equal("/v1/accounts/A-TEST0001/", handler.SentRequests[0].RequestUri?.AbsolutePath);
    }

    [Fact]
    public async Task GetAsync_WhenAccountNumberHasSpecialCharacters_EscapesPath()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.OK, FixtureFile.Read("accounts-detail.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await client.Accounts.GetAsync("A/TEST", cancellationToken: CancellationToken.None);

        Assert.Contains("A%2FTEST", handler.SentRequests[0].RequestUri?.AbsoluteUri, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetAsync_WhenAccountNumberNull_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Accounts.GetAsync(null!, cancellationToken: CancellationToken.None));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_WhenAccountNumberWhitespace_ThrowsBeforeHttp()
    {
        QueuedHttpMessageHandler handler = new();

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        await Assert.ThrowsAsync<OctopusEnergyRequestException>(
            () => client.Accounts.GetAsync("   ", cancellationToken: CancellationToken.None));

        Assert.Empty(handler.SentRequests);
    }

    [Fact]
    public async Task GetAsync_WhenUnauthorized_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.Unauthorized, FixtureFile.Read("accounts-unauthorized.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None));

        Assert.Equal(HttpStatusCode.Unauthorized, exception.StatusCode);
        Assert.Equal("Authentication credentials were not provided.", exception.Detail);
    }

    [Fact]
    public async Task GetAsync_WhenForbidden_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.Forbidden, FixtureFile.Read("accounts-forbidden.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None));

        Assert.Equal(HttpStatusCode.Forbidden, exception.StatusCode);
        Assert.Equal("You do not have permission to perform this action.", exception.Detail);
    }

    [Fact]
    public async Task GetAsync_WhenNotFound_ThrowsOctopusEnergyApiException()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(HttpStatusCode.NotFound, FixtureFile.Read("accounts-not-found.json"));

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        OctopusEnergyApiException exception = await Assert.ThrowsAsync<OctopusEnergyApiException>(
            () => client.Accounts.GetAsync("A-MISSING", cancellationToken: CancellationToken.None));

        Assert.Equal(HttpStatusCode.NotFound, exception.StatusCode);
        Assert.Equal("Not found.", exception.Detail);
    }

    [Fact]
    public async Task GetAsync_WhenPropertyListsNull_DeserialisesEmptyLists()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.OK,
            """{"number":"A-TEST0001","properties":null}""");

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        Account account = await client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None);

        Assert.NotNull(account.Properties);
        Assert.Empty(account.Properties);
    }

    [Fact]
    public async Task GetAsync_WhenNestedMeterPointListsNull_DeserialisesEmptyLists()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.OK,
            """
            {
              "number": "A-TEST0001",
              "properties": [
                {
                  "id": 9000001,
                  "address_line_1": "1 Example Street",
                  "town": "LONDON",
                  "postcode": "W1 1AA",
                  "electricity_meter_points": null,
                  "gas_meter_points": null
                }
              ]
            }
            """);

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        Account account = await client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None);

        AccountProperty property = Assert.Single(account.Properties);
        Assert.NotNull(property.ElectricityMeterPoints);
        Assert.Empty(property.ElectricityMeterPoints);
        Assert.NotNull(property.GasMeterPoints);
        Assert.Empty(property.GasMeterPoints);
    }

    [Fact]
    public async Task GetAsync_WhenMeterPointHasEmptyMeters_DeserialisesEmptyMeterList()
    {
        QueuedHttpMessageHandler handler = new();
        handler.Enqueue(
            HttpStatusCode.OK,
            """
            {
              "number": "A-TEST0001",
              "properties": [
                {
                  "id": 9000001,
                  "address_line_1": "1 Example Street",
                  "town": "LONDON",
                  "postcode": "W1 1AA",
                  "electricity_meter_points": [
                    {
                      "mpan": "1000000000999",
                      "profile_class": 8,
                      "consumption_standard": 2900,
                      "meters": [],
                      "agreements": [
                        {
                          "tariff_code": "E-1R-VAR-22-11-01-N",
                          "valid_from": "2023-07-25T00:00:00+01:00",
                          "valid_to": null
                        }
                      ],
                      "is_export": true
                    }
                  ],
                  "gas_meter_points": []
                }
              ]
            }
            """);

        using HttpClient httpClient = CreateHttpClient(handler);
        using OctopusEnergyClient client = new(httpClient);

        Account account = await client.Accounts.GetAsync("A-TEST0001", cancellationToken: CancellationToken.None);

        ElectricityMeterPoint exportPoint = Assert.Single(account.Properties[0].ElectricityMeterPoints);
        Assert.Equal("1000000000999", exportPoint.Mpan);
        Assert.True(exportPoint.IsExport);
        Assert.NotNull(exportPoint.Meters);
        Assert.Empty(exportPoint.Meters);
        Assert.Single(exportPoint.Agreements);
    }

    private static HttpClient CreateHttpClient(QueuedHttpMessageHandler handler)
    {
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.example.test/v1/"),
        };
    }
}
