using KeepGrouped.API.AiBackend.AiClient;
using KeepGrouped.API.Middlewares;
using System.Threading.RateLimiting;
using Microsoft.Extensions.Options;


namespace KeepGrouped.API.AiBackend;

static class AiBackendBuilder
{
	public static void BuildAiBackend(this WebApplicationBuilder builder)
	{
		var myOptions = new RateLimitOptions();

		builder.Services.AddOptions<AiBackendOptions>()
			.Bind(builder.Configuration.GetSection(AiBackendOptions.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		var serviceUrl = builder.Configuration["Ai:ServiceUrl"]
			?? throw new InvalidOperationException("Ai:ServiceUrl is missing");

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
		builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
		{
			client.BaseAddress = new Uri(serviceUrl);
		});

		builder.Services.AddHttpClient("ai-back", client =>
		{
			client.BaseAddress = new Uri(serviceUrl);
		});
	}
}