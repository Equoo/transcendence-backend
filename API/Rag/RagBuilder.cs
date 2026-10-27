using KeepGrouped.API.Events;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Storage;

public static class RagBuilder
{
    public static void BuildRag(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<RagService>();
        builder.Services.AddSingleton<RagQueue>();
        builder.Services.AddKeyedScoped<RagHandler, RagEvent>(typeof(Event));
        builder.Services.AddKeyedScoped<RagHandler, RagFile>(typeof(StorageFile));
    }
}