using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Storage;

public static class DeleteFileEndpoint
{
    public static void MapDeleteFile(this IEndpointRouteBuilder group)
    {
        group.MapDelete("/{key}", [Authorize] async (DeleteFileCommand command, string key) =>
        {
            var result = await command.ExecuteAsync(key);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.NoContent();
        })
        .WithName("files.delete")
        .WithSummary("Delete a file")
        .WithDescription("Deletes the file from storage and removes its metadata record.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status502BadGateway);
    }
}
