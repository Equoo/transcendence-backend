using System.Threading.RateLimiting;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.RateLimiting;

public static class RateLimitBuilder
{
	public static void BuildRateLimiting(this WebApplicationBuilder builder)
	{
		var opts = builder.Configuration
			.GetSection(RateLimitOptions.SectionName)
			.Get<RateLimitOptions>() ?? new();

		builder.Services.AddRateLimiter(options =>
		{
			options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

			options.AddPolicy<string>(RateLimitPolicies.AiChat, ctx =>
			{
				var userId = ctx.RequestServices.GetRequiredService<TokenContext>().User.Id;

				return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
				{
					TokenLimit = opts.AiChat.TokenLimit,
					QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
					QueueLimit = opts.AiChat.QueueLimit,
					ReplenishmentPeriod = TimeSpan.FromSeconds(opts.AiChat.ReplenishmentPeriod),
					TokensPerPeriod = opts.AiChat.TokensPerPeriod,
					AutoReplenishment = opts.AiChat.AutoReplenishment
				});
			});

			options.AddPolicy<string>(RateLimitPolicies.Login, ctx =>
			{
				var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";

				return RateLimitPartition.GetSlidingWindowLimiter(ip, _ => new SlidingWindowRateLimiterOptions
				{
					PermitLimit = opts.Login.PermitLimit,
					Window = TimeSpan.FromSeconds(opts.Login.WindowSeconds),
					SegmentsPerWindow = opts.Login.SegmentsPerWindow,
					QueueLimit = 0
				});
			});
		});
	}
}