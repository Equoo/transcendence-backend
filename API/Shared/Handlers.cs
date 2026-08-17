namespace KeepGrouped.API;

public interface IHandler;

static public class HandlerRegistration
{
    static public void AddHandlers(this WebApplicationBuilder builder)
    {
        var handlers = typeof(Program).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IHandler).IsAssignableFrom(t));

        foreach (var handler in handlers)
        {
            builder.Services.AddScoped(handler);
        }
    }
}
