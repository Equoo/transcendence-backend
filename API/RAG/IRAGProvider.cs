using KeepGrouped.API.Storage;

namespace KeepGrouped.API.RAG;

public interface IRAGProvider
{
    public IStorage _storage { get; init; }
    public bool EmbedFile(Storage.File file);
    public Stream Prompt(string prompt);
}