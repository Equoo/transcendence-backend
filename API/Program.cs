using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<KeepGroupedDb>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
                .UseSeeding(
                    (db, _) =>
                    {
                        if (db.Set<User>().FirstOrDefault(u => u.UserName == "asventi") != null)
                        {
                            return;
                        }
                        User user = new("asventi");
                        db.Set<User>().Add(user);
                        db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
                        db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
                        db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
                        db.Set<EventRole>().Add(new EventRole() { Name = "Any" });
                        db.SaveChanges();

                        var ev = new Event()
                        {
                            Name = "Default Event",
                            Date = DateTime.UtcNow.AddMinutes(30),
                            Location = "Default Location",
                            Size = 10,
                            Organizer = user,
                            // EventRoles = [.. db.Set<EventRole>()]
                        };
                        ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "DPS"));
                        ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "Heal"));
                        ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "Any"));
                        db.Set<Event>().Add(ev);

                        db.SaveChanges();
                    }
                )
        );
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValidation();
        builder.Services.AddAuthorization();
        builder.Services.AddSignalR();
        builder.Services.AddControllers();
        builder
            .Services.AddIdentity<User, IdentityRole>()
            .AddEntityFrameworkStores<KeepGroupedDb>();
        // builder.Services.AddScoped<IPasswordHasher<User>, >();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen();
        }

        var app = builder.Build();
        app.UseStatusCodePages();
        using var serviceScope = app.Services.CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
        if (app.Environment.IsDevelopment())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseAuthorization();
        }
        else
        {
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

        app.MapHub<ChatHub>("/chat");
        app.MapEvents();
        app.MapRegistrations();
        app.MapUsers();
        app.MapEventRoles();
        app.MapChat();
        app.Run();
    }
}
