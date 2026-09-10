using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeepGrouped.API.Storage;

public record UploadFileRequest
{
    [Required]
    [Length(1, 255)]
    public string Name { get; init; } = null!;

    [Required]
    public IFormFile File { get; init; } = null!;
}

public record UploadFileResponse(
    string Key,
    string Name,
    long Length,
    string ETag,
    string ContentType,
    DateTime LastUpdated,
    UserSummary Creator
)
{
    public static UploadFileResponse FromEntity(StorageFile file) => new(
        file.Key, file.Name, file.Length, file.ETag, file.ContentType,
        file.LastUpdated, UserSummary.FromEntity(file.Creator));
}

public static class UploadFileEndpoint
{
    public static void MapUploadFile(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", [Authorize] async (UploadFileCommand command, TokenContext token, [FromForm] UploadFileRequest req) =>
        {
            var result = await command.ExecuteAsync(
                req.File.OpenReadStream(), req.Name, req.File.ContentType, req.File.Length, token.User);

            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.CreatedAtRoute("files.get", new { key = result.Value.Key }, UploadFileResponse.FromEntity(result.Value));
        })
        .DisableAntiforgery()
        .WithName("files.upload")
        .WithSummary("Upload a file")
        .WithDescription("Uploads a file as `multipart/form-data` (fields `name` and `file`) and stores its metadata in the database.")
        .Accepts<UploadFileRequest>("multipart/form-data")
        .Produces<UploadFileResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
        .ProducesProblem(StatusCodes.Status502BadGateway);
    }
}
