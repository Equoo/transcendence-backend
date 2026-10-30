using KeepGrouped.API.AiBackend.AiClient;

namespace KeepGrouped.API.AiBackend;

static class AiBackendBuilder
{
	public static void BuildAiBackend(this WebApplicationBuilder builder)
	{
		builder.Services.AddOptions<AiBackendOptions>()
			.Bind(builder.Configuration.GetSection(AiBackendOptions.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		var serviceUrl = builder.Configuration["Ai:ServiceUrl"]
			?? throw new InvalidOperationException("Ai:ServiceUrl is missing");

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