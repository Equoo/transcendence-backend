using KeepGrouped.API.Chat;
using System.Text.Json.Serialization;
using KeepGrouped.API.AiBackend;
using KeepGrouped.API.Events;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;
using KeepGrouped.API.Users.Auth;
using KeepGrouped.API.Users.Invitation;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace KeepGrouped.API;

class Program
{
	static void Main(string[] args)
	{
		// ------------ Buildings Dependances

		var builder = WebApplication.CreateBuilder(args);

		var tokenPolicy = "token";
		var myOptions = new RateLimitOptions();
		builder.Configuration.GetSection(RateLimitOptions.RateLimit).Bind(myOptions);

		builder.Services.AddRateLimiter(_ => _
		.AddTokenBucketLimiter(policyName: tokenPolicy, options =>
		{
			options.TokenLimit = myOptions.TokenLimit;
			options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
			options.QueueLimit = myOptions.QueueLimit;
			options.ReplenishmentPeriod = TimeSpan.FromSeconds(myOptions.ReplenishmentPeriod);
			options.TokensPerPeriod = myOptions.TokensPerPeriod;
			options.AutoReplenishment = myOptions.AutoReplenishment;
		}));
		builder.BuildStorage();
		builder.BuildDb();
		builder.BuildAuthentication();
		builder.AddHandlers();

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		builder.Services.AddProblemDetails();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddValidation();
		builder.Services.AddSignalR();

		builder.Services.AddProblemDetails();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddValidation();

		builder.Services.AddHttpClient<IApiClient, ApiClient>(client =>
		{
			client.BaseAddress = new Uri("http://localhost:3000");
		});
		builder.Services.AddScoped<IAiBackendClient, AiBackendClient>();
		if (builder.Environment.IsDevelopment())
		{
			builder.Services.AddSwaggerGen();
		}

		var app = builder.Build();
		app.UseForwardedHeaders();
		app.UseStatusCodePages();
		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
			using var serviceScope = app.Services.CreateScope();
			var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
			context.Database.Migrate();
		}
		else
		{
			using var serviceScope = app.Services.CreateScope();
			var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
			context.Database.Migrate();
		}

		app.UseRateLimiter();
		app.MapGet("/", () => "Hello World from API!")
			.WithTags("Diagnostics")
			.WithName("root")
			.WithSummary("API root")
			.WithDescription("Returns a constant greeting, used to check that the API is up.")
			.Produces<string>(StatusCodes.Status200OK);

		app.MapEvents();
		app.MapRegistrations();
		app.MapUsers();
		app.MapMe();
		app.MapEventRoles();
		app.MapStorageFiles();
		app.MapAuthentication();
		app.MapInvitations();
		app.MapRoles();
		app.MapHub<KeepGroupedHub>("");
		app.MapAiBackend();
		app.MapChannels();
		app.MapMessages();
		app.Run();
	}
}
