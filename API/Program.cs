using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContextPool<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        var app = builder.Build();
        app.MapGet("/", () => "Hello World from API!");
        app.MapGet("/test/{name}", async (string name, KeepGroupedDb db) =>
        {
            User newuser = new()
            {
                Id = Random.Shared.Next(),
                Name = name
            };
            db.Users.Add(newuser);
            await db.SaveChangesAsync();

            return Results.Created();
        });

        app.MapGet("/test", async (KeepGroupedDb db) =>
        {
            return await db.Users.ToListAsync();
        });

        app.Run();

    }
}
