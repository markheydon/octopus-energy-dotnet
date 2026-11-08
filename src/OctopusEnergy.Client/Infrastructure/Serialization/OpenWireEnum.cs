namespace OctopusEnergy.Client.Infrastructure.Serialization;

internal static class OpenWireEnum
{
    internal static TEnum UnknownValue<TEnum>()
        where TEnum : struct, Enum
    {
        return Enum.Parse<TEnum>("Unknown", ignoreCase: false);
    }

    internal static bool IsUnknown<TEnum>(TEnum value)
        where TEnum : struct, Enum
    {
        return EqualityComparer<TEnum>.Default.Equals(value, UnknownValue<TEnum>());
    }

    internal static WireField<ProductEnergyDirection> ParseProductEnergyDirection(string? wire)
    {
        if (string.IsNullOrEmpty(wire))
        {
            return new WireField<ProductEnergyDirection> { Value = ProductEnergyDirection.Unknown };
        }

        if (string.Equals(wire, "IMPORT", StringComparison.OrdinalIgnoreCase))
        {
            return new WireField<ProductEnergyDirection> { Value = ProductEnergyDirection.Import };
        }

        if (string.Equals(wire, "EXPORT", StringComparison.OrdinalIgnoreCase))
        {
            return new WireField<ProductEnergyDirection> { Value = ProductEnergyDirection.Export };
        }

        return new WireField<ProductEnergyDirection>
        {
            Value = ProductEnergyDirection.Unknown,
            UnparsedValue = wire,
        };
    }

    internal static string? ToWire(ProductEnergyDirection value) =>
        value switch
        {
            ProductEnergyDirection.Import => "IMPORT",
            ProductEnergyDirection.Export => "EXPORT",
            _ => null,
        };

    internal static WireField<MeterRegisterRate> ParseMeterRegisterRate(string? wire)
    {
        if (string.IsNullOrEmpty(wire))
        {
            return new WireField<MeterRegisterRate> { Value = MeterRegisterRate.Unknown };
        }

        if (string.Equals(wire, "STANDARD", StringComparison.Ordinal))
        {
            return new WireField<MeterRegisterRate> { Value = MeterRegisterRate.Standard };
        }

        if (string.Equals(wire, "DAY", StringComparison.Ordinal))
        {
            return new WireField<MeterRegisterRate> { Value = MeterRegisterRate.Day };
        }

        if (string.Equals(wire, "NIGHT", StringComparison.Ordinal))
        {
            return new WireField<MeterRegisterRate> { Value = MeterRegisterRate.Night };
        }

        return new WireField<MeterRegisterRate>
        {
            Value = MeterRegisterRate.Unknown,
            UnparsedValue = wire,
        };
    }

    internal static string? ToWire(MeterRegisterRate value) =>
        value switch
        {
            MeterRegisterRate.Standard => "STANDARD",
            MeterRegisterRate.Day => "DAY",
            MeterRegisterRate.Night => "NIGHT",
            _ => null,
        };

    internal static WireField<ProductPaymentMethod> ParseProductPaymentMethod(string? wire)
    {
        if (string.IsNullOrEmpty(wire))
        {
            return new WireField<ProductPaymentMethod> { Value = ProductPaymentMethod.Unknown };
        }

        if (string.Equals(wire, "direct_debit_monthly", StringComparison.OrdinalIgnoreCase))
        {
            return new WireField<ProductPaymentMethod> { Value = ProductPaymentMethod.DirectDebitMonthly };
        }

        if (string.Equals(wire, "direct_debit_quarterly", StringComparison.OrdinalIgnoreCase))
        {
            return new WireField<ProductPaymentMethod> { Value = ProductPaymentMethod.DirectDebitQuarterly };
        }

        if (string.Equals(wire, "non_direct_debit", StringComparison.OrdinalIgnoreCase))
        {
            return new WireField<ProductPaymentMethod> { Value = ProductPaymentMethod.NonDirectDebit };
        }

        return new WireField<ProductPaymentMethod>
        {
            Value = ProductPaymentMethod.Unknown,
            UnparsedValue = wire,
        };
    }

    internal static string? ToWire(ProductPaymentMethod value) =>
        value switch
        {
            ProductPaymentMethod.DirectDebitMonthly => "direct_debit_monthly",
            ProductPaymentMethod.DirectDebitQuarterly => "direct_debit_quarterly",
            ProductPaymentMethod.NonDirectDebit => "non_direct_debit",
            _ => null,
        };
}
