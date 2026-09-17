
public interface IRateLimiter
{
	Task<bool> TryAcquireAsync(string userId, int tokens = 1, CancellationToken cancellationToken = default);
}