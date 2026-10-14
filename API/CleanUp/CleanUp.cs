
using Amazon.S3.Model;
using KeepGrouped.API;
using KeepGrouped.API.Events;
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

    public void DeleteDb<T>(KeepGroupedDb db) where T : class, IExpirable
    {
        db.Set<T>().Where(t => t.ExpireAt < DateTime.UtcNow).ExecuteDelete();
    }

    private async void DoWork(object? state)
    {

        await using var scope = scopes.CreateAsyncScope();
        KeepGroupedDb? context = scope.ServiceProvider.GetService<KeepGroupedDb>();


        var handlers = typeof(Program).Assembly.GetTypes()
           .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IExpirable).IsAssignableFrom(t));



        foreach (var handler in handlers)
        {

            MethodInfo? methodInfo = GetType().GetMethod(nameof(DeleteDb));

            methodInfo.MakeGenericMethod(handler).Invoke(this, [context]);
        }
    }

}

// Rajoute la class dans la liste data comme ca dans le dowork on fais une boucle qui recupere une liste de toute les
// class present dans data imaginons on mets dans data la class Event | User | Invitation lists alors on
// Boucle pour tout recuperer et check le expirate 

// Dans chaque class concerne rajouter un attribut expirates si il est present alors le rajouter automatiquement dans
// la variable data dans cleanups

