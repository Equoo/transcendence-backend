using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Events;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Users;

namespace KeepGrouped.API.Storage;

public class StorageFile : IRagProcessable
{
    [Key]
    public string Key { get; set; } = null!;

    string IRagProcessable.RagId => Key;

    public string Name { get; set; } = null!;

    public long Length { get; set; }

    public string ETag { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public User Creator { get; set; } = null!;

    public ICollection<Event> Events { get; } = [];
    public bool Ingested { get; set; } = false;
}

public record FileSummary(string Key, string Name, long Length, string ContentType)
{
    public static FileSummary FromEntity(StorageFile file) => new(file.Key, file.Name, file.Length, file.ContentType);
}
