using System.Net.Mime;
using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KeepGrouped.API.Storage;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Amazon.Runtime;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOptions<StorageOptions>()
            .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
            .ValidateDataAnnotations().ValidateOnStart();
        builder.Services.AddScoped<IStorage, Garage>();
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
            var s3config = new AmazonS3Config()
            {
                ServiceURL = options.ServiceUrl,
                AuthenticationRegion = options.Region,
                ForcePathStyle = true,
                RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
            };

            var creds = new BasicAWSCredentials(options.AccessKey, options.SecretKey);
            return new AmazonS3Client(creds, s3config);
        });
        builder.Services.AddDbContext<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSeeding((db, _) =>
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
        app.MapEventRoles();
        app.MapStorage();
        app.Run();

    }
}
