namespace OctopusEnergy.Client;

/// <summary>
/// Payment method for product catalogue tariffs.
/// </summary>
public enum ProductPaymentMethod
{
    /// <summary>
    /// Direct debit billed monthly.
    /// </summary>
    DirectDebitMonthly,

    /// <summary>
    /// Direct debit billed quarterly.
    /// </summary>
    DirectDebitQuarterly,

    /// <summary>
    /// Non-direct-debit payment.
    /// </summary>
    NonDirectDebit,
}
