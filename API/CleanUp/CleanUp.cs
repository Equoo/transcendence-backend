
using Amazon.S3.Model;
using KeepGrouped.API;
using KeepGrouped.API.Events;
using KeepGrouped.API.Rag;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.OpenApi;
using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace Microsoft.EntityFrameworkCore;

public class CleanUp(IServiceScopeFactory scopes) : BackgroundService
{
    private PeriodicTimer? _timer = null;
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _timer = new PeriodicTimer(TimeSpan.FromHours(2));

        while (await _timer.WaitForNextTickAsync(stoppingToken))
        {
            await using var scope = scopes.CreateAsyncScope();
            KeepGroupedDb? context = scope.ServiceProvider.GetService<KeepGroupedDb>();

            RagQueue? rag = scope.ServiceProvider.GetService<RagQueue>();

            var handlers = typeof(Program).Assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IExpirable).IsAssignableFrom(t));

            MethodInfo? methodInfo = GetType().GetMethod(nameof(DeleteDb));

            if (methodInfo is not null)
            {
                foreach (var handler in handlers)
                {
                    await (Task)methodInfo.MakeGenericMethod(handler).Invoke(this, [context, rag])!;
                }
            }

        }

    }

    public async Task DeleteDb<T>(KeepGroupedDb db, RagQueue rag) where T : class, IExpirable
    {
        Console.WriteLine("\n\n\n\n\n" + DateTime.UtcNow + "\n\n\n\n\n");

        await db.Set<T>().Where(t => t.ExpireAt < DateTime.UtcNow).ExecuteDeleteAsync();
    }
}


