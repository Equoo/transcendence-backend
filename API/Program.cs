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
using Microsoft.AspNetCore.HttpOverrides;
using KeepGrouped.API.Password;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TokenContext = KeepGrouped.API.Users.TokenContext;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        // ------------ Buildings Dependances

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
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        });

        
        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            if (builder.Environment.IsDevelopment())
            {
                options.Password.RequiredLength = 0;    
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }
        })
        .AddEntityFrameworkStores<KeepGroupedDb>();
        
        builder.Services.AddProblemDetails();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddValidation();
        builder.Services.AddScoped<IPasswordHasher<User>, KeepGroupedPasswordHasher>();
        builder.Services.AddScoped<TokenContext>();
        
       
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = "KeepGrouped",
                ValidAudience = "KeepGrouped",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"))
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    context.Token = context.Request.Cookies["AuthToken"];
                    return Task.CompletedTask;
                },
            };
        });

        builder.Services.AddAuthorization();
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.AddSwaggerGen();
        }

        // ------------ Prepare the app

        var app = builder.Build();
        app.UseForwardedHeaders();
        app.UseStatusCodePages();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<TokenContextMiddleware>();
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
        app.MapGet("/", () => "Hello World from API!")
            .WithTags("Diagnostics")
            .WithName("root")
            .WithSummary("API root")
            .WithDescription("Returns a constant greeting, used to check that the API is up.")
            .Produces<string>(StatusCodes.Status200OK);
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
        app.MapAuthentication();
        
        // ------------ Start the app

        app.Run();

    }
}
