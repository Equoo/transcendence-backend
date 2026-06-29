using System.Net.Mime;
using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSeeding((db, _) =>
        {
            User user = new("asventi");
            if (db.Set<User>().FirstOrDefault(u => u.UserName == "asventi") == null)
            {
                db.Set<User>().Add(user);
            }
            if (db.Set<Event>().FirstOrDefault(e => e.Name == "Default Event") == null)
            {
                db.Set<Event>().Add(new Event()
                {
                    Name = "Default Event",
                    Date = DateTime.UtcNow.AddMinutes(30),
                    Location = "Default Location",
                    Size = 10,
                    Organizer = user
                });
            }
            db.SaveChanges();
        }));
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValidation();
        builder.Services.AddAuthorization();
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<KeepGroupedDb>();
        // builder.Services.AddScoped<IPasswordHasher<User>, >();
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
            app.UseAuthorization();

        }
        else
        {
            using var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
            context.Database.Migrate();
        }
        app.MapGet("/", () => "Hello World from API!");
        // app.MapPost("/register", async ([FromForm] string username, KeepGroupedDb db) =>
        // {
        //     var user = new ApplicationUser(username);
        //     db.Add(user);
        //     await db.SaveChangesAsync();
        //     return user;
        // }).DisableAntiforgery();

        app.MapEvents();
        app.MapRegistrations();
        app.MapUsers();
        app.Run();

    }
}
