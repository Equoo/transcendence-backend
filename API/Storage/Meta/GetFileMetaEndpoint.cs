using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Storage;

public record GetFileMetaResponse(
    string Key,
    string Name,
    long Length,
    string ETag,
    string ContentType,
    DateTime LastUpdated,
    UserSummary Creator
)
{
    public static GetFileMetaResponse FromEntity(StorageFile file) => new(
        file.Key, file.Name, file.Length, file.ETag, file.ContentType,
        file.LastUpdated, UserSummary.FromEntity(file.Creator));
}

public static class GetFileMetaEndpoint
{
    public static void MapGetFileMeta(this IEndpointRouteBuilder group)
    {
        group.MapGet("/meta/{key}", [Authorize] async (GetFileMetaQuery query, string key) =>
        {
            var result = await query.ExecuteAsync(key);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("meta.files.get")
        .WithSummary("Get a file metadata")
        .WithDescription("Get information of a file without downloading it.")
        .Produces<GetFileMetaResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
