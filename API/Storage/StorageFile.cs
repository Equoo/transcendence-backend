using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Events;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace KeepGrouped.API.Storage;

public class StorageFile
{
    [Key]
    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long Length { get; set; }

    public string ETag { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public User Creator { get; set; } = null!;

    public ICollection<Event> Events { get; } = [];
}

public record FileResponse(string Key, string Name, long Length, string ETag, string ContentType, DateTime LastUpdated, UserResponse Creator)
{
    public static FileResponse FromEntity(StorageFile file) => new(file.Key, file.Name, file.Length, file.ETag, file.ContentType,
        file.LastUpdated, UserResponse.FromEntity(file.Creator));
}

public record FileUploadRequest
{
    [Required]
    public string Name { get; init; } = null!;

    [Required]
    public IFormFile File { get; init; } = null!;
}

public static class StorageFileEndpoints
{
    public static void MapStorageFiles(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/files").WithTags("Files");

        group.MapPost("/", [Authorize] async (IStorage storage, KeepGroupedDb db, TokenContext token, [FromForm] FileUploadRequest req) =>
        {
            var res = await storage.UploadAsync(req.File.OpenReadStream(), req.File.ContentType);
            if ((int)res.Code >= 400)
            {
                return Results.StatusCode((int)res.Code);
            }

            var filedb = new StorageFile()
            {
                Key = res.Key,
                Name = req.Name,
                ETag = res.ETag,
                ContentType = req.File.ContentType,
                Length = req.File.Length,
                LastUpdated = DateTime.UtcNow,
                Creator = token.User
            };

            db.Files.Add(filedb);

            await db.SaveChangesAsync();
            return Results.CreatedAtRoute("files.get", new { key = res.Key }, FileResponse.FromEntity(filedb));
        })
        .DisableAntiforgery()
        .WithName("files.upload")
        .WithSummary("Upload a file")
        .WithDescription("Uploads a file as `multipart/form-data` (fields `name` and `file`) and stores its metadata in the database.")
        .Accepts<FileUploadRequest>("multipart/form-data")
        .Produces<FileResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{key}", [Authorize] async (IStorage storage, string key, KeepGroupedDb db) =>
        {
            var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
            if (filedb is null)
            {
                return Results.NotFound();
            }
            var res = await storage.DownloadAsync(key);
            if ((int)res.Code >= 400)
            {
                return Results.StatusCode((int)res.Code);
            }
            return Results.Stream(res.Stream, filedb.ContentType, filedb.Name,
                filedb.LastUpdated, new EntityTagHeaderValue(filedb.ETag));
        })
        .WithName("files.get")
        .WithSummary("Download a file")
        .WithDescription("Streams the content of the file identified by its key.")
        .Produces(StatusCodes.Status200OK, contentType: "application/octet-stream")
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/meta/{key}", [Authorize] async (IStorage storage, string key, KeepGroupedDb db) =>
        {
            var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
            if (filedb is null)
            {
                return Results.NotFound();
            }

            return Results.Ok(FileResponse.FromEntity(filedb));
        })
        .WithName("meta.files.get")
        .WithSummary("Get a file metadata")
        .WithDescription("Get information of a file without downloading it.")
        .Produces<FileResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/", [Authorize] async (KeepGroupedDb db) =>
        {
            var files = await db.Files.ToListAsync();
            return Results.Ok(files.Select(FileResponse.FromEntity));
        })
        .WithName("files.list")
        .WithSummary("List files")
        .WithDescription("Returns the metadata of every uploaded file, without their content.")
        .Produces<IEnumerable<FileResponse>>(StatusCodes.Status200OK);

        group.MapDelete("/{key}", [Authorize] async (IStorage storage, string key, KeepGroupedDb db) =>
        {
            var filedb = await db.Files.SingleOrDefaultAsync(f => f.Key == key);
            if (filedb is null)
            {
                return Results.NotFound();
            }

            var res = await storage.DeleteAsync(key);
            if ((int)res.Code >= 400)
            {
                return Results.StatusCode((int)res.Code);
            }

            db.Files.Remove(filedb);
            await db.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("files.delete")
        .WithSummary("Delete a file")
        .WithDescription("Deletes the file from storage and removes its metadata record.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
