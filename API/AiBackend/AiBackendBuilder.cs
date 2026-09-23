using Microsoft.AspNetCore.RateLimiting;
using KeepGrouped.API.Middlewares;
using System.Threading.RateLimiting;
using System.Text.Json.Serialization;

namespace KeepGrouped.API.AiBackend;

static class AiBackendBuilder
{
	public static void BuildAiBackend(this WebApplicationBuilder builder)
	{
		var myOptions = new RateLimitOptions();

		builder.Configuration.GetSection(RateLimitOptions.RateLimit).Bind(myOptions);
		builder.Services.AddRateLimiter(options =>
		{
			options.AddPolicy<string>("ai-chat", httpContext =>
			{
				var tokenContext = httpContext.RequestServices.GetRequiredService<TokenContext>();
				var userId = tokenContext.User.Id;

				return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
				{
					TokenLimit = myOptions.TokenLimit,
					QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
					QueueLimit = myOptions.QueueLimit,
					ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod),
					TokensPerPeriod = myOptions.TokensPerPeriod,
					AutoReplenishment = myOptions.AutoReplenishment
				});
			});
		});
		builder.Services.AddScoped<IAiBackendClient, AiBackendClient>();
		builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
		{
			client.BaseAddress = new Uri("http://ai-back-dev:7070");
		});
	}
}