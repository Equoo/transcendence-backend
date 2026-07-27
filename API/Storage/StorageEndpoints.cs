using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;


namespace KeepGrouped.API.Storage;

public class File
{
    [Key]
    public string Key { get; set; } = null!;

    public string Name { get; set; } = null!;

    public long Length { get; set; }

    public string ETag { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public DateTime LastUpdated { get; set; }

    public User Creator { get; set; } = null!;
}

public record FileResponse(string Key, string Name, long Length, string ETag, string ContentType, DateTime LastUpdated, UserResponse Creator)
{
    public static FileResponse FromEntity(File file) => new(file.Key, file.Name, file.Length, file.ETag, file.ContentType,
        file.LastUpdated, UserResponse.FromEntity(file.Creator));
}

public class FileUploadRequest
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public IFormFile File { get; set; } = null!;
}

public static class StorageEndpoints
{
    public static void MapStorage(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/files").WithTags("Files");

        group.MapPost("/", async (IStorage storage, KeepGroupedDb db, [FromForm] FileUploadRequest req) =>
        {
            // Replace with authentication
            User? user = await db.Users.FirstOrDefaultAsync(u => u.UserName == "asventi");

            if (user is null)
            {
                return Results.Unauthorized();
            }

            var res = await storage.UploadAsync(req.File.OpenReadStream(), req.File.ContentType);
            if ((int)res.Code >= 400)
            {
                return Results.StatusCode((int)res.Code);
            }

            var filedb = new File()
            {
                Key = res.Key,
                Name = req.Name,
                ETag = res.ETag,
                ContentType = req.File.ContentType,
                Length = req.File.Length,
                LastUpdated = DateTime.UtcNow,
                Creator = user
            };

            db.Files.Add(filedb);

            await db.SaveChangesAsync();
            return Results.CreatedAtRoute("files.get", new { key = res.Key }, FileResponse.FromEntity(filedb));
        })
        .DisableAntiforgery()
        .WithName("files.upload")
        .WithSummary("Upload a file")
        .WithDescription("Uploads a file as `multipart/form-data` (fields `name` and `file`) and stores its metadata in the database.")
        .Produces<FileResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/{key}", async (IStorage storage, string key, KeepGroupedDb db) =>
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

        group.MapGet("/", async (KeepGroupedDb db) =>
        {
            var files = await db.Files.ToListAsync();
            return Results.Ok(files.Select(FileResponse.FromEntity));
        })
        .WithName("files.list")
        .WithSummary("List files")
        .WithDescription("Returns the metadata of every uploaded file, without their content.")
        .Produces<IEnumerable<FileResponse>>(StatusCodes.Status200OK);

        group.MapDelete("/{key}", async (IStorage storage, string key, KeepGroupedDb db) =>
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