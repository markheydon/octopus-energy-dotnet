namespace OctopusEnergy.Client;

/// <summary>
/// Documented REST <c>page_size</c> defaults and maxima from the Octopus API.
/// </summary>
public static class RestPageSizeLimits
{
    /// <summary>
    /// Default <c>page_size</c> when callers omit the parameter.
    /// </summary>
    public const int Default = 100;

    /// <summary>
    /// Maximum <c>page_size</c> for unit-rate and standing-charge list endpoints.
    /// </summary>
    public const int RatesMaximum = 1_500;

    /// <summary>
    /// Maximum <c>page_size</c> for consumption list endpoints.
    /// </summary>
    public const int ConsumptionMaximum = 25_000;

    /// <summary>
    /// Validates that <paramref name="pageSize"/> is within documented REST limits.
    /// </summary>
    /// <param name="pageSize">The requested page size.</param>
    /// <param name="maximum">The documented maximum for the endpoint.</param>
    /// <exception cref="OctopusEnergyRequestException">
    /// Thrown when <paramref name="pageSize"/> is less than 1 or exceeds <paramref name="maximum"/>.
    /// </exception>
    public static void Validate(int pageSize, int maximum)
    {
        if (pageSize < 1)
        {
            throw new OctopusEnergyRequestException(
                $"page_size {pageSize} must be at least 1.");
        }

        if (pageSize > maximum)
        {
            throw new OctopusEnergyRequestException(
                $"page_size {pageSize} exceeds the documented maximum of {maximum}.");
        }
    }
}
