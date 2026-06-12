using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // builder.Services.AddAuthorization();
        // builder.Services.AddAuthentication();
        builder.Services.AddDbContextPool<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<KeepGroupedDb>().Database.Migrate();
        }
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapGet("/", () => "Hello World from API!");
        app.MapPost("/register", async ([FromForm] string username, KeepGroupedDb db) =>
        {
            var user = new ApplicationUser(username);
            db.Add(user);
            await db.SaveChangesAsync();
            return user;
        }).DisableAntiforgery();

        app.Run();

    }
}
