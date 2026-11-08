using OctopusEnergy.Client.Infrastructure.GraphQL;
using OctopusEnergy.Client.Models.Octoplus;

namespace OctopusEnergy.Client.Services.Octoplus;

/// <summary>
/// GraphQL Octoplus enrolment, loyalty, and wheel-of-fortune helpers for the authenticated dashboard API key.
/// </summary>
public sealed class OctoplusService : IOctoplusService
{
    private readonly GraphQlClient? _graphQl;

    internal OctoplusService(GraphQlClient? graphQl)
    {
        _graphQl = graphQl;
    }

    /// <inheritdoc />
    public async Task<OctoplusAccountInfo> GetAccountInfoAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        OctoplusAccountInfoQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
        };

        OctoplusAccountInfoQueryData data = await _graphQl!.ExecuteAsync<OctoplusAccountInfoQueryData>(
            OctoplusQueries.OctoplusAccountInfo,
            variables,
            cancellationToken).ConfigureAwait(false);

        OctoplusAccountInfoWire? wire = data.OctoplusAccountInfo
            ?? throw new OctopusEnergyParseException(
                "The Octoplus account info query returned no data for the supplied account number.");

        return new OctoplusAccountInfo
        {
            EnrollmentStatus = wire.EnrollmentStatus,
            IsLoyaltyPointsUser = wire.IsLoyaltyPointsUser,
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<LoyaltyPointLedgerEntry>> ListLoyaltyPointLedgersAsync(
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();

        LoyaltyPointLedgersQueryData data = await _graphQl!.ExecuteAsync<LoyaltyPointLedgersQueryData>(
            OctoplusQueries.LoyaltyPointLedgers,
            variables: null,
            cancellationToken).ConfigureAwait(false);

        if (data.LoyaltyPointLedgers is not { Count: > 0 })
        {
            return [];
        }

        return data.LoyaltyPointLedgers.Select(MapLedger).ToList();
    }

    /// <inheritdoc />
    public async Task<LoyaltyPointsBalance> GetLoyaltyPointsBalanceAsync(
        string accountNumber,
        string? schemeCode = null,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        LoyaltyPointsBalanceQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
            SchemeCode = string.IsNullOrWhiteSpace(schemeCode) ? null : schemeCode.Trim(),
        };

        LoyaltyPointsBalanceQueryData data = await _graphQl!.ExecuteAsync<LoyaltyPointsBalanceQueryData>(
            OctoplusQueries.LoyaltyPointsBalance,
            variables,
            cancellationToken).ConfigureAwait(false);

        LoyaltyPointsBalanceWire? wire = data.LoyaltyPointsBalance
            ?? throw new OctopusEnergyParseException(
                "The loyalty points balance query returned no data for the supplied account number.");

        return new LoyaltyPointsBalance
        {
            Points = wire.LoyaltyPoints,
            TotalMonetaryAmountPence = wire.TotalMonetaryAmount,
        };
    }

    /// <inheritdoc />
    public async Task<WheelOfFortuneSpinsAvailability> GetWheelOfFortuneSpinsAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        WheelOfFortuneSpinsQueryVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
        };

        WheelOfFortuneSpinsQueryData data = await _graphQl!.ExecuteAsync<WheelOfFortuneSpinsQueryData>(
            OctoplusQueries.WheelOfFortuneSpins,
            variables,
            cancellationToken).ConfigureAwait(false);

        WheelOfFortuneSpinsWire? wire = data.WheelOfFortuneSpins
            ?? throw new OctopusEnergyParseException(
                "The wheel-of-fortune query returned no data for the supplied account number.");

        return new WheelOfFortuneSpinsAvailability
        {
            Electricity = MapFuelSpins(wire.Electricity),
            Gas = MapFuelSpins(wire.Gas),
        };
    }

    /// <inheritdoc />
    public async Task<JoinOctoplusCampaignResult> JoinCampaignAsync(
        string accountNumber,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        JoinOctoplusCampaignMutationVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
        };

        JoinOctoplusCampaignMutationData data = await _graphQl!.ExecuteAsync<JoinOctoplusCampaignMutationData>(
            OctoplusQueries.JoinOctoplusCampaign,
            variables,
            cancellationToken).ConfigureAwait(false);

        bool? hasEnrolled = data.JoinOctoplusCampaign?.Outcome?.Octoplus?.HasEnrolled;

        return new JoinOctoplusCampaignResult
        {
            HasEnrolled = hasEnrolled,
        };
    }

    /// <inheritdoc />
    public async Task<RedeemLoyaltyPointsResult> RedeemPointsForAccountCreditAsync(
        string accountNumber,
        int points,
        string? schemeCode = null,
        CancellationToken cancellationToken = default)
    {
        EnsureGraphQl();
        ValidateAccountNumber(accountNumber);

        if (points <= 0)
        {
            throw new OctopusEnergyRequestException("Points to redeem must be greater than zero.");
        }

        RedeemLoyaltyPointsMutationVariables variables = new()
        {
            AccountNumber = accountNumber.Trim(),
            Points = points,
            SchemeCode = string.IsNullOrWhiteSpace(schemeCode) ? null : schemeCode.Trim(),
        };

        RedeemLoyaltyPointsMutationData data = await _graphQl!.ExecuteAsync<RedeemLoyaltyPointsMutationData>(
            OctoplusQueries.RedeemLoyaltyPointsForAccountCredit,
            variables,
            cancellationToken).ConfigureAwait(false);

        return new RedeemLoyaltyPointsResult
        {
            PointsRedeemed = data.RedeemLoyaltyPointsForAccountCredit?.PointsRedeemed,
        };
    }

    private static LoyaltyPointLedgerEntry MapLedger(LoyaltyPointLedgerEntryWire wire)
    {
        return new LoyaltyPointLedgerEntry
        {
            Id = wire.Id,
            LedgerType = wire.LedgerType,
            Value = wire.Value,
            BalanceCarriedForward = wire.BalanceCarriedForward,
            PostedAt = wire.PostedAt,
            AccountNumber = wire.AccountNumber,
        };
    }

    private static FuelSpinsAvailability? MapFuelSpins(FuelSpinsWire? wire)
    {
        if (wire is null)
        {
            return null;
        }

        return new FuelSpinsAvailability
        {
            MaxSpinsPerMonth = wire.MaxSpinsPerMonth,
            RemainingSpinsThisMonth = wire.RemainingSpinsThisMonth,
            UsedSpinsThisMonth = wire.UsedSpinsThisMonth,
        };
    }

    private void EnsureGraphQl()
    {
        if (_graphQl is null)
        {
            throw new OctopusEnergyRequestException(
                "A dashboard API key is required to query Octoplus via GraphQL.");
        }
    }

    private static void ValidateAccountNumber(string accountNumber)
    {
        if (string.IsNullOrWhiteSpace(accountNumber))
        {
            throw new OctopusEnergyRequestException("Account number is required.");
        }
    }
}
