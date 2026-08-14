namespace KeepGrouped.API;

/// <summary>
/// Marks a use case handler. Implementations are registered as scoped services by
/// <see cref="HandlerRegistration.AddHandlers"/> and injected straight into minimal-API delegates.
/// </summary>
public interface IHandler;

static public class HandlerRegistration
{
    /// <summary>
    /// Registers every <see cref="IHandler"/> of this assembly as a scoped service. Registration has to
    /// happen at builder time: minimal APIs ask <c>IServiceProviderIsService</c> whether a complex
    /// delegate parameter is a service or the request body, so an unregistered handler makes the
    /// endpoint fail at startup with "Body was inferred but the method does not allow inferred body parameters".
    /// </summary>
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
