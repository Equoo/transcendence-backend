using KeepGrouped.API.AiBackend.Delete;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Roles;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Storage;

public static class DeleteFileEndpoint
{
    public static void MapDeleteFile(this IEndpointRouteBuilder group)
    {
        group.MapDelete("/{key}", [Authorize][Roles(Perms.HandleKnowledge)] async (DeleteFileCommand fileCmd, string key) =>
        {
            var fileResult = await fileCmd.ExecuteAsync(key);
            if (fileResult.IsProblem)
            {
                return fileResult.Problem;
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
