namespace KeepGrouped.API.Storage;

public class StorageOptions
{
    public const string SectionName = "Storage";

    public required string ServiceUrl { get; init; }
    public required string Region { get; init; }
    public required string BucketName { get; init; }
    public required string AccessKey { get; init; }
    public required string SecretKey { get; init; }
}