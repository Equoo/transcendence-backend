using System.Net.Mime;
using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using KeepGrouped.API.Tests;
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
            db.Set<ApplicationUser>().Add(new ApplicationUser("asventi"));
            db.Set<Event>().Add(new Event() { Name = "Default Event", Date = DateTime.UtcNow.AddMinutes(30), Location = "Default Location", Size = 10 });
            db.SaveChanges();
        }));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter()
                        .AddEndpointsApiExplorer()
                        .AddSwaggerGen()
                        .AddValidation()
                        .AddAuthorization()
                        .AddIdentityApiEndpoints<ApplicationUser>()
                        .AddEntityFrameworkStores<KeepGroupedDb>();

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            var serviceScope = app.Services.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();

        }
        else
        {
            var serviceScope = app.Services.CreateScope();
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

        EventEndpoints.Map(app);
        RegistrationEndpoints.Map(app);
        
        TestEndpoint.Map(app);
        app.Run();

    }
}
