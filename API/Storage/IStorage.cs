using System.Net;

namespace KeepGrouped.API.Storage;

public record UploadResponse(string Key, string ETag, HttpStatusCode Code);
public record DownloadResponse(Stream Stream, HttpStatusCode Code);
public record DeleteResponse(HttpStatusCode Code);

public interface IStorage
{
    Task<UploadResponse> UploadAsync(Stream stream, string contentType, CancellationToken ct = default);
    Task<DownloadResponse> DownloadAsync(string key, CancellationToken ct = default);
    Task<DeleteResponse> DeleteAsync(string key, CancellationToken ct = default);
}
