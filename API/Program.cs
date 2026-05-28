namespace KeepGrouped;

class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var app = builder.Build();

        app.MapGet("/", () => "Hello World from API!");
        app.MapGet("/api", () => "Hello World from API Path!");

        app.Run();

    }
}
