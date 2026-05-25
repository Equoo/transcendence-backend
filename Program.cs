var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapGet("/", () => "Hello World from API!");
app.MapGet("/api", () => "Hello World from API Path!");

app.Run();
