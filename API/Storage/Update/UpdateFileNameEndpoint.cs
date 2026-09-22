using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Storage;

public record UpdateFileNameRequest
{
    [Required]
    [Length(1, 255)]
    public string Name { get; init; } = null!;
}

public static class UpdateFileNameEndpoint
{
    public static void MapUpdateFileName(this IEndpointRouteBuilder group)
    {
        group.MapPatch("/{key}/name", [Authorize] async (UpdateFileNameCommand query, string key, [FromBody] UpdateFileNameRequest req) =>
        {
            var result = await query.ExecuteAsync(key, req.Name);
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("files.name.patch")
        .WithSummary("Rename a file")
        .WithDescription("Changes the display name of a stored file without touching its content.")
        .Produces<GetFileMetaResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
