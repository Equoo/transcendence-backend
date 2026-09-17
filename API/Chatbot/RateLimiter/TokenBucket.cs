
public class TokenBucket
{
	private readonly int _capacity;
	private readonly int _refillRate;
	private double _tokens;
	private DateTime _lastRefill;
	private readonly object _lock = new object();

	public TokenBucket(int capacity, int refillRate)
	{
		_capacity = capacity;
		_refillRate = refillRate;
		_tokens = capacity;
		_lastRefill = DateTime.UtcNow;
	}

	public bool TryConsume(int tokens)
	{
		lock (_lock)
		{
			RefillTokens();
			if (_tokens >= tokens)
			{
				_tokens -= tokens;
				return true;
			}
			return false;
		}
	}

	private void RefillTokens()
	{
		var now = DateTime.UtcNow;
		var elapsedSeconds = (now - _lastRefill).TotalSeconds;
		var tokensToAdd = elapsedSeconds * _refillRate;
		_tokens = Math.Min(_capacity, _tokens + tokensToAdd);
		_lastRefill = now;
	}

}