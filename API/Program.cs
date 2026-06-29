using System.Net.Mime;
using KeepGrouped.API.Events;
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
        builder.Services.AddDbContext<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSeeding((db, _) =>
        {
            if (db.Set<ApplicationUser>().FirstOrDefault(u => u.UserName == "asventi") == null)
            {
                db.Set<ApplicationUser>().Add(new ApplicationUser("asventi"));
            }
            if (db.Set<Event>().FirstOrDefault(e => e.Name == "Default Event") == null)
            {
                db.Set<Event>().Add(new Event() { Name = "Default Event", Date = DateTime.UtcNow.AddMinutes(30), Location = "Default Location", Size = 10 });
            }
            db.SaveChanges();
        }));
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValidation();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen();
        }

        var app = builder.Build();
        app.UseStatusCodePages();
        if (app.Environment.IsDevelopment())
        {
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
            context.Database.Migrate();
        }
        app.MapGet("/", () => "Hello World from API!");
        app.MapPost("/register", async ([FromBody] string username, KeepGroupedDb db) =>
        {
            var user = new ApplicationUser(username);
            db.Add(user);
            await db.SaveChangesAsync();
            return user;
        });

        EventEndpoints.Map(app);
        RegistrationEndpoints.Map(app);
        app.Run();

    }
}
