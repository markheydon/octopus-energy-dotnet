namespace OctopusEnergy.Client;

/// <summary>
/// Payment method for product catalogue tariffs.
/// </summary>
public enum ProductPaymentMethod
{
    /// <summary>
    /// Direct debit billed monthly.
    /// </summary>
    /// <remarks>Wire token: <c>direct_debit_monthly</c>.</remarks>
    DirectDebitMonthly,

    /// <summary>
    /// Direct debit billed quarterly.
    /// </summary>
    /// <remarks>Wire token: <c>direct_debit_quarterly</c>.</remarks>
    DirectDebitQuarterly,

    /// <summary>
    /// Non-direct-debit payment.
    /// </summary>
    /// <remarks>Wire token: <c>non_direct_debit</c>.</remarks>
    NonDirectDebit,

    /// <summary>
    /// Unrecognised or missing wire token.
    /// </summary>
    Unknown,
}
