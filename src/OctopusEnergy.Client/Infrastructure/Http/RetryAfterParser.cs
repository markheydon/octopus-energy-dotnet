using System.Globalization;
using System.Net.Http.Headers;

namespace OctopusEnergy.Client.Infrastructure.Http;

internal static class RetryAfterParser
{
    internal static TimeSpan GetDelay(
        HttpResponseMessage response,
        int attemptIndex,
        OctopusEnergyRetryOptions options)
    {
        if (TryGetRetryAfterDelay(response.Headers, out TimeSpan retryAfterDelay))
        {
            return ClampDelay(retryAfterDelay, options.MaxDelay);
        }

        double multiplier = Math.Pow(2, attemptIndex);
        TimeSpan backoff = TimeSpan.FromTicks((long)(options.BaseDelay.Ticks * multiplier));
        return ClampDelay(backoff, options.MaxDelay);
    }

    private static bool TryGetRetryAfterDelay(HttpResponseHeaders headers, out TimeSpan delay)
    {
        if (!headers.TryGetValues("Retry-After", out IEnumerable<string>? values))
        {
            delay = default;
            return false;
        }

        string? value = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(value))
        {
            delay = default;
            return false;
        }

        if (int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int seconds))
        {
            delay = TimeSpan.FromSeconds(Math.Max(0, seconds));
            return true;
        }

        if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset retryAt))
        {
            delay = retryAt - DateTimeOffset.UtcNow;
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            return true;
        }

        delay = default;
        return false;
    }

    private static TimeSpan ClampDelay(TimeSpan delay, TimeSpan maxDelay)
    {
        if (delay < TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        if (delay > maxDelay)
        {
            return maxDelay;
        }

        return delay;
    }
}
