
public class RateLimitOptions
{
	public const string RateLimit = "RateLimit";
	public int TokenLimit { get; set; }
	public int QueueLimit { get; set; }
	public double ReplenishmentPeriod { get; set; }
	public int TokensPerPeriod { get; set; }
	public bool AutoReplenishment { get; set; }
}