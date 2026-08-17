using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;

namespace KeepGrouped.API.Storage;

public static class DownloadFileEndpoint
{
    public static void MapDownloadFile(this IEndpointRouteBuilder group)
    {
        group.MapGet("/{key}", [Authorize] async (DownloadFileQuery query, string key) =>
        {
            var result = await query.ExecuteAsync(key);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            var file = result.Value;
            return Results.Stream(file.Content, file.ContentType, file.FileName,
                file.LastModified, new EntityTagHeaderValue(file.ETag));
        })
        .WithName("files.get")
        .WithSummary("Download a file")
        .WithDescription("Streams the content of the file identified by its key.")
        .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway);
    }
}
