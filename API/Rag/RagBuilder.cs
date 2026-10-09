using KeepGrouped.API.Rag;

public static class RagBuilder
{
    public static void BuildRag(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<RagService>();
        builder.Services.AddSingleton<RagQueue>();
        builder.Services.AddScoped<RagHandler, RagEvent>();
    }
}