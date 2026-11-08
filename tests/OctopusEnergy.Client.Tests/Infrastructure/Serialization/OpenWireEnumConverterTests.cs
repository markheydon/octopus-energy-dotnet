using System.Text.Json;
using OctopusEnergy.Client;
using OctopusEnergy.Client.Infrastructure.Serialization;
using OctopusEnergy.Client.Models.Accounts;
using OctopusEnergy.Client.Models.Products;

namespace OctopusEnergy.Client.Tests.Infrastructure.Serialization;

public sealed class OpenWireEnumConverterTests
{
    private static readonly JsonSerializerOptions Options = new()
    {
        TypeInfoResolver = OctopusEnergyJsonContext.Default,
    };

    [Theory]
    [InlineData("IMPORT", ProductEnergyDirection.Import)]
    [InlineData("import", ProductEnergyDirection.Import)]
    [InlineData("EXPORT", ProductEnergyDirection.Export)]
    public void Deserialize_ProductDirection_KnownTokens_MapToEnum(string wire, ProductEnergyDirection expected)
    {
        Product product = JsonSerializer.Deserialize<Product>($$"""{"code":"X","direction":"{{wire}}"}""", Options)!;

        Assert.NotNull(product.Direction);
        Assert.Equal(expected, product.Direction.Value.Value);
        Assert.Null(product.Direction.Value.UnparsedValue);
    }

    [Fact]
    public void Deserialize_ProductDirection_UnknownToken_PreservesWireValue()
    {
        Product product = JsonSerializer.Deserialize<Product>("""{"code":"X","direction":"FUTURE"}""", Options)!;

        Assert.NotNull(product.Direction);
        Assert.Equal(ProductEnergyDirection.Unknown, product.Direction.Value.Value);
        Assert.Equal("FUTURE", product.Direction.Value.UnparsedValue);
    }

    [Fact]
    public void Deserialize_ProductDirection_NullJson_ReturnsNullProperty()
    {
        Product product = JsonSerializer.Deserialize<Product>("""{"code":"X","direction":null}""", Options)!;

        Assert.Null(product.Direction);
    }

    [Theory]
    [InlineData("STANDARD", MeterRegisterRate.Standard)]
    [InlineData("DAY", MeterRegisterRate.Day)]
    [InlineData("NIGHT", MeterRegisterRate.Night)]
    public void Deserialize_MeterRegisterRate_KnownTokens_MapToEnum(string wire, MeterRegisterRate expected)
    {
        MeterRegister register = JsonSerializer.Deserialize<MeterRegister>($$"""{"identifier":"1","rate":"{{wire}}"}""", Options)!;

        Assert.Equal(expected, register.Rate.Value);
    }

    [Fact]
    public void Deserialize_MeterRegisterRate_UnknownToken_PreservesWireValue()
    {
        MeterRegister register = JsonSerializer.Deserialize<MeterRegister>("""{"identifier":"1","rate":"ECO7_DAY"}""", Options)!;

        Assert.Equal(MeterRegisterRate.Unknown, register.Rate.Value);
        Assert.Equal("ECO7_DAY", register.Rate.UnparsedValue);
    }

    [Theory]
    [InlineData("direct_debit_monthly", ProductPaymentMethod.DirectDebitMonthly)]
    [InlineData("DIRECT_DEBIT_MONTHLY", ProductPaymentMethod.DirectDebitMonthly)]
    [InlineData("non_direct_debit", ProductPaymentMethod.NonDirectDebit)]
    public void Deserialize_TariffChargePaymentMethod_KnownTokens_MapToEnum(string wire, ProductPaymentMethod expected)
    {
        TariffCharge charge = JsonSerializer.Deserialize<TariffCharge>(
            $$"""{"value_exc_vat":1,"value_inc_vat":1,"valid_from":"2020-01-01T00:00:00Z","payment_method":"{{wire}}"}""",
            Options)!;

        Assert.NotNull(charge.PaymentMethod);
        Assert.Equal(expected, charge.PaymentMethod.Value.Value);
    }

    [Fact]
    public void Deserialize_TariffChargePaymentMethod_UnknownToken_PreservesWireValue()
    {
        TariffCharge charge = JsonSerializer.Deserialize<TariffCharge>(
            """{"value_exc_vat":1,"value_inc_vat":1,"valid_from":"2020-01-01T00:00:00Z","payment_method":"future_payment"}""",
            Options)!;

        Assert.NotNull(charge.PaymentMethod);
        Assert.Equal(ProductPaymentMethod.Unknown, charge.PaymentMethod.Value.Value);
        Assert.Equal("future_payment", charge.PaymentMethod.Value.UnparsedValue);
    }
}
