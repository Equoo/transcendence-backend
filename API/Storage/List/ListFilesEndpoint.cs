using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Storage;

public record ListFilesResponse(
    string Key,
    string Name,
    long Length,
    string ContentType,
    DateTime LastUpdated,
    UserSummary Creator
)
{
    public static ListFilesResponse FromEntity(StorageFile file) => new(
        file.Key, file.Name, file.Length, file.ContentType,
        file.LastUpdated, UserSummary.FromEntity(file.Creator));
}

public static class ListFilesEndpoint
{
    public static void MapListFiles(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", [Authorize] async (ListFilesQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("files.list")
        .WithSummary("List files")
        .WithDescription("Returns the metadata of every uploaded file, without their content.")
        .Produces<IEnumerable<ListFilesResponse>>(StatusCodes.Status200OK);
    }
}
