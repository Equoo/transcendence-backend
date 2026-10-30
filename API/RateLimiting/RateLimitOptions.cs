namespace KeepGrouped.API.RateLimiting;

public class RateLimitOptions
{
	public const string SectionName = "RateLimit";

	public TokenBucketPolicyOptions AiChat { get; set; } = new();
	public SlidingWindowPolicyOptions Login { get; set; } = new();
}

public class TokenBucketPolicyOptions
{
	public int TokenLimit { get; set; }
	public int QueueLimit { get; set; }
	public double ReplenishmentPeriod { get; set; }
	public int TokensPerPeriod { get; set; }
	public bool AutoReplenishment { get; set; }
}

public class SlidingWindowPolicyOptions
{
	public int PermitLimit { get; set; }
	public double WindowSeconds { get; set; }
	public int SegmentsPerWindow { get; set; }
}