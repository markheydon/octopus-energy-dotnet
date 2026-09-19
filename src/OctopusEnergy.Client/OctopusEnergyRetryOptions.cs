namespace OctopusEnergy.Client;

/// <summary>
/// Configures bounded retry behaviour for transient REST HTTP failures (429 and 503).
/// </summary>
public sealed class OctopusEnergyRetryOptions
{
    internal const int MaxAttemptsUpperBound = 10;

    /// <summary>
    /// Conservative default: retry enabled with up to three retry attempts after the first failure.
    /// </summary>
    public static OctopusEnergyRetryOptions Default { get; } = new();

    /// <summary>
    /// Disables SDK retry; non-success HTTP responses throw immediately.
    /// </summary>
    public static OctopusEnergyRetryOptions Disabled { get; } = new() { Enabled = false };

    /// <summary>
    /// Gets or sets whether the SDK retries transient HTTP 429 and 503 responses.
    /// </summary>
    public bool Enabled { get; init; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts after the first failed request.
    /// </summary>
    /// <remarks>
    /// A value of <c>3</c> allows up to four HTTP calls in total (one initial plus three retries).
    /// </remarks>
    public int MaxAttempts { get; init; } = 3;

    /// <summary>
    /// Gets or sets the upper bound for delay between retry attempts.
    /// </summary>
    public TimeSpan MaxDelay { get; init; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Gets or sets the base delay used for exponential backoff when the API omits <c>Retry-After</c>.
    /// </summary>
    public TimeSpan BaseDelay { get; init; } = TimeSpan.FromSeconds(1);

    internal void Validate()
    {
        if (MaxAttempts < 0 || MaxAttempts > MaxAttemptsUpperBound)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MaxAttempts),
                MaxAttempts,
                $"MaxAttempts must be between 0 and {MaxAttemptsUpperBound}.");
        }

        if (BaseDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(BaseDelay), BaseDelay, "BaseDelay must not be negative.");
        }

        if (MaxDelay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(MaxDelay), MaxDelay, "MaxDelay must not be negative.");
        }
    }
}
