
namespace KeepGrouped.API.Middlewares;

public class DelayMiddleware
{
    private readonly RequestDelegate _next;

    public DelayMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST")
        {
            Thread.Sleep(10);
        }
        await _next(context);
        return;
    }

}