using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
<<<<<<< HEAD
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;
using KeepGrouped.API.Users.Invitation;
using Microsoft.AspNetCore.HttpOverrides;
=======
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity;
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API;

class Program
{
	static void Main(string[] args)
	{
		// ------------ Buildings Dependances

<<<<<<< HEAD
		var builder = WebApplication.CreateBuilder(args);

		builder.BuildStorage();
		builder.BuildDb();
		builder.BuildAuthentication();
		builder.AddHandlers();

		builder.Services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
		});

		builder.Services.AddProblemDetails();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddValidation();

		if (builder.Environment.IsDevelopment())
		{
			builder.Services.AddSwaggerGen();
		}

		var app = builder.Build();
		app.UseForwardedHeaders();
		app.UseStatusCodePages();
		using var serviceScope = app.Services.CreateScope();
		var context = serviceScope.ServiceProvider.GetRequiredService<KeepGroupedDb>();
		if (app.Environment.IsDevelopment())
		{
			context.Database.EnsureDeleted();
			context.Database.EnsureCreated();
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		else
		{
			context.Database.Migrate();
		}
		app.MapGet("/", () => "Hello World from API!")
			.WithTags("Diagnostics")
			.WithName("root")
			.WithSummary("API root")
			.WithDescription("Returns a constant greeting, used to check that the API is up.")
			.Produces<string>(StatusCodes.Status200OK);

		app.MapHub<ChatHub>("/chat");
		app.MapEvents();
		app.MapRegistrations();
		app.MapUsers();
		app.MapMe();
		app.MapEventRoles();
		app.MapStorageFiles();
		app.MapAuthentication();
		app.MapInvitations();

		app.MapChat();
		app.Run();
	}
=======
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
                        User user2 = new("equo");
                        db.Set<User>().Add(user2);

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
        builder.Services.AddScoped<TokenContext>();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen();
        }

        var app = builder.Build();
        app.UseMiddleware<TokenContextMiddleware>(); // NOTE: PLACEHOLDER
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

        app.MapHub<ChatHub>("/chat");
        app.MapEvents();
        app.MapRegistrations();
        app.MapUsers();
        app.MapEventRoles();
        app.MapChat();
        app.Run();
    }
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
}
