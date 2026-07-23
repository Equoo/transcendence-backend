using System.ComponentModel.DataAnnotations;
using System.Drawing;
using Amazon.S3.Model;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;


namespace KeepGrouped.API.Storage;

public class File
{
    [Key]
    public string Key { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public long Length { get; set; }

    [Required]
    public string ETag { get; set; } = null!;

    [Required]
    public string ContentType { get; set; } = null!;

    [Required]
    public DateTime LastUpdated { get; set; }

    [Required]
    public User Creator = null!;
}

public record FileResponse(string Key, string Name, long Length, string ETag, string ContentType, DateTime LastUpdated, UserResponse Creator)
{
    public static FileResponse FromEntity(File file) => new(file.Key, file.Name, file.Length, file.ETag, file.ContentType,
        file.LastUpdated, UserResponse.FromEntity(file.Creator));
}

public static class StorageEndpoints
{
    public static void MapStorage(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("files/");

        group.MapPost("/", async (IStorage storage, KeepGroupedDb db, [FromForm] string name, IFormFile file) =>
        {
            // Replace with authentication
            User? user = await db.Users.FirstOrDefaultAsync(u => u.UserName == "asventi");

            if (user is null)
            {
                return Results.Unauthorized();
            }
            var res = await storage.UploadAsync(file.OpenReadStream());
            if ((int)res.Code >= 400)
            {
                return Results.StatusCode((int)res.Code);
            }
            var filedb = new File()
            {
                Key = res.Key,
                Name = name,
                ETag = res.ETag,
                ContentType = file.ContentType,
                Length = file.Length,
                LastUpdated = DateTime.UtcNow,
                Creator = user
            };

            db.Files.Add(filedb);

            await db.SaveChangesAsync();
            // return TypedResults.CreatedAtRoute($"{key}", "files/", new { key });
            return Results.Ok(res.Key);
        }).DisableAntiforgery();

        group.MapGet("{key}/", async (IStorage storage, string key, KeepGroupedDb db) =>
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
        });

        group.MapGet("/", async (KeepGroupedDb db) =>
        {
            var files = await db.Files.ToListAsync();
            return Results.Ok(files.Select(FileResponse.FromEntity));
        });
    }
}