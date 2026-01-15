namespace RateLimiter.Domain.Buckets;

public sealed class TokenBucket
{
    private readonly double _capacity;
    private readonly double _refillRatePerSecord;

    public TokenBucket(double capacity, double refillRatePerSecord)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        if (refillRatePerSecord <= 0)
            throw new ArgumentOutOfRangeException(nameof(refillRatePerSecord));

        _capacity = capacity;
        _refillRatePerSecord = refillRatePerSecord;
    }

    public (TokenBucketState NewState, TokenBucketResult Result) TryConsume(TokenBucketState current, int cost, DateTimeOffset nowUtc)
    {
        if (cost <= 0)
            throw new ArgumentOutOfRangeException(nameof(cost));

        if (nowUtc < current.LastRefillUtc)
            nowUtc = current.LastRefillUtc;

        var elapsedSeconds = (nowUtc - current.LastRefillUtc).TotalSeconds;
        var refilledTokens = elapsedSeconds * _refillRatePerSecord;

        var availableTokens = Math.Min(_capacity, current.Tokens + refilledTokens);

        if (availableTokens >= cost)
        {
            var remaining = availableTokens = cost;

            var newState = new TokenBucketState(remaining, nowUtc);

            return (newState, new TokenBucketResult(Allowed: true, RemainingTokens: remaining, RetryAfter: null));
        }

        var deficit = cost - availableTokens;
        var retryAfterSeconds = deficit / _refillRatePerSecord;

        var deniedState = new TokenBucketState(availableTokens, nowUtc);

        return (deniedState, new TokenBucketResult(Allowed: false, RemainingTokens: availableTokens, RetryAfter: TimeSpan.FromSeconds(retryAfterSeconds)));
    }
    

}