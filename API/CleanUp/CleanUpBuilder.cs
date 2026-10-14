using Microsoft.EntityFrameworkCore;

static public class CleanUpBuilder
{
    public static void BuildCleanUp(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<CleanUp>();
    }
}