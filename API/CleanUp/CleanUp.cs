
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
    private Timer? _timer = null;
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }

    public async void DeleteDb<T>(KeepGroupedDb db, RagQueue rag) where T : class, IExpirable
    {

        List<T>? list = await db.Set<T>().Where(t => t.ExpireAt < DateTime.UtcNow).ToListAsync();

        foreach (T element in list)
        {

            db.Set<T>().Remove(element);

            await db.SaveChangesAsync();

        }
    }

    private async void DoWork(object? state)
    {

        await using var scope = scopes.CreateAsyncScope();
        KeepGroupedDb? context = scope.ServiceProvider.GetService<KeepGroupedDb>();

        RagQueue? rag = scope.ServiceProvider.GetService<RagQueue>();

        var handlers = typeof(Program).Assembly.GetTypes()
           .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IExpirable).IsAssignableFrom(t));

        foreach (var handler in handlers)
        {

            MethodInfo? methodInfo = GetType().GetMethod(nameof(DeleteDb));

            methodInfo.MakeGenericMethod(handler).Invoke(this, [context, rag]);
        }
    }

}

// Rajoute la class dans la liste data comme ca dans le dowork on fais une boucle qui recupere une liste de toute les
// class present dans data imaginons on mets dans data la class Event | User | Invitation lists alors on
// Boucle pour tout recuperer et check le expirate 

// Dans chaque class concerne rajouter un attribut expirates si il est present alors le rajouter automatiquement dans
// la variable data dans cleanups

