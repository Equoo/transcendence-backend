namespace KeepGrouped.API.Rag;

public abstract class RagHandler
{
    public abstract string Kind { get; }

    public ValueTask ProcessOperation(string id, RagOp op, CancellationToken ct = default)
    {
        switch (op)
        {
            case RagOp.Upsert:
                return Upsert(id, ct);
            case RagOp.Delete:
                return Delete(id, ct);
        }
        return new ValueTask();
    }
    public abstract ValueTask Upsert(string id, CancellationToken ct = default);
    public abstract ValueTask Delete(string id, CancellationToken ct = default);
}

public sealed class RagService(RagQueue queue, IServiceScopeFactory scopes, ILogger<RagService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await foreach (RagItem item in queue.Reader.ReadAllAsync(ct))
        {
            RagOp op = queue.Dequeue(item);
            try
            {
                await using var scope = scopes.CreateAsyncScope();
                RagHandler handler = scope.ServiceProvider.GetServices<RagHandler>().Single((ha) => ha.Kind == item.Kind);
                await handler.ProcessOperation(item.Id, op, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Rag processing failed for {Item}: {Operation}", item, op);
            }
        }
    }
}