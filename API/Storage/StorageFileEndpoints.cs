namespace KeepGrouped.API.Storage;

public static class StorageFileEndpoints
{
    public static void MapStorageFiles(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/files").WithTags("Files");

        group.MapUploadFile();
        group.MapDownloadFile();
        group.MapGetFileMeta();
        group.MapListFiles();
        group.MapDeleteFile();
    }
}
