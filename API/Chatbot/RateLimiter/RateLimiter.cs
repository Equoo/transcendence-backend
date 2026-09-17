using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

public class RateLimiter : IRateLimiter
{
	private readonly ConcurrentDictionary<string, TokenBucket> _buckets = new();
	private int Capacity { get; }
	private int RefillRate { get; }

	public RateLimiter(IOptions<RateLimiterOptions> options)
	{
		Capacity = options.Value.Capacity;
		RefillRate = options.Value.RefillRate;
	}

	public Task<bool> TryAcquireAsync(string userId, int tokens = 1, CancellationToken cancellationToken = default)
	{
		var bucket = _buckets.GetOrAdd(userId, _ => new TokenBucket(Capacity, RefillRate));
		return Task.FromResult(bucket.TryConsume(tokens));
	}
}