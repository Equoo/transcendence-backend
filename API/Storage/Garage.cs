using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace KeepGrouped.API.Storage;

public class Garage(IAmazonS3 s3, IOptions<StorageOptions> options, KeepGroupedDb db) : IStorage
{
    private readonly StorageOptions _options = options.Value;
    private readonly KeepGroupedDb _db = db;
    private readonly IAmazonS3 _s3 = s3;

    async Task<UploadResponse> IStorage.UploadAsync(Stream stream, string contentType, CancellationToken ct)
    {
        var key = Guid.CreateVersion7().ToString();

        var req = new PutObjectRequest
        {
            InputStream = stream,
            AutoCloseStream = true,
            BucketName = _options.BucketName,
            Key = key,
            ContentType = contentType,
        };
        var res = await _s3.PutObjectAsync(req, ct);

        return new UploadResponse(key, res.ETag, res.HttpStatusCode);
    }

    async Task<DownloadResponse> IStorage.DownloadAsync(string key, CancellationToken ct)
    {
        var req = new GetObjectRequest()
        {
            BucketName = _options.BucketName,
            Key = key,
        };

        var obj = await _s3.GetObjectAsync(req, ct);

        return new DownloadResponse(obj.ResponseStream, obj.HttpStatusCode);
    }

    async Task<DeleteResponse> IStorage.DeleteAsync(string key, CancellationToken ct)
    {
        var req = new DeleteObjectRequest()
        {
            BucketName = _options.BucketName,
            Key = key
        };

        var res = await _s3.DeleteObjectAsync(req, ct);

        return new DeleteResponse(res.HttpStatusCode);
    }
}