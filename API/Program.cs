using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization();
        builder.Services.AddAuthentication();
        builder.Services.AddDbContextPool<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddEntityFrameworkStores<KeepGroupedDb>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<KeepGroupedDb>().Database.Migrate();
            }
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapIdentityApi<ApplicationUser>();
        app.MapGet("/", () => "Hello World from API!");

        app.Run();

    }
}
