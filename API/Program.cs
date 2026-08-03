using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KeepGrouped.API.Storage;
using Amazon.S3;
using Microsoft.Extensions.Options;
using Amazon.Runtime;
using Microsoft.AspNetCore.HttpOverrides;
using KeepGrouped.API.Password;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KeepGrouped.API.Middlewares;
using TokenContext = KeepGrouped.API.Middlewares.TokenContext;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        // ------------ Buildings Dependances

        var builder = WebApplication.CreateBuilder(args);

        builder.BuildStorage();
        builder.BuildDb();
        builder.BuildAuthentication();

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

        app.MapEvents();
        app.MapRegistrations();
        app.MapUsers();
        app.MapEventRoles();
        app.MapStorage();
        app.MapAuthentication();

        app.Run();

    }
}
