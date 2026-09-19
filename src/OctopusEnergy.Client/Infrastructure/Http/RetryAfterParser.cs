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
        RetryConditionHeaderValue? retryAfter = headers.RetryAfter;
        if (retryAfter is null)
        {
            delay = default;
            return false;
        }

        if (retryAfter.Delta is TimeSpan delta)
        {
            delay = delta < TimeSpan.Zero ? TimeSpan.Zero : delta;
            return true;
        }

        if (retryAfter.Date is DateTimeOffset retryAt)
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
