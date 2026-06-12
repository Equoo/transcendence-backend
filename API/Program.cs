using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

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
        builder.Services.AddHttpLogging();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        using (var serviceScope = app.Services.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
            context.Database.Migrate();
            context.Database.EnsureCreated();
            RelationalDatabaseCreator databaseCreator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();
            databaseCreator.CreateTables();
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

        EventEndpoints.Map(app);
        app.Run();

    }
}
