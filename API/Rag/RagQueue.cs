
using System.Threading.Channels;

namespace KeepGrouped.API.Rag;

public enum RagOp { Upsert, Delete };

public record RagItem(Type Type, string Id);

public class RagQueue(ILogger<RagQueue> logger)
{
    private readonly Channel<RagItem> _channel = Channel.CreateUnbounded<RagItem>();
    private readonly Dictionary<RagItem, RagOp> _pending = [];
    private readonly object _lock = new();

    public ChannelReader<RagItem> Reader => _channel.Reader;

    public void Upsert<T>(T item) where T : IRagProcessable => Enqueue(new RagItem(typeof(T), item.RagId), RagOp.Upsert);
    public void Delete<T>(T item) where T : IRagProcessable => Enqueue(new RagItem(typeof(T), item.RagId), RagOp.Delete);

    private void Enqueue(RagItem item, RagOp op)
    {
        lock (_lock)
        {
            bool alreadyQueued = _pending.ContainsKey(item);
            _pending[item] = op;
            if (!alreadyQueued)
            {
                if (_channel.Writer.TryWrite(item))
                {
                    logger.LogInformation("Enqueued {Item}: {Operation}", item, op);
                }
                else
                {
                    logger.LogError("Can't queue {Item}: {Operation}", item, op);
                }
                return;
            }
            logger.LogInformation("Replaced operation of {Item}: {Operation}", item, op);
        }
    }

    public RagOp Dequeue(RagItem item)
    {
        lock (_lock)
        {
            var op = _pending[item];
            _pending.Remove(item);
            logger.LogDebug("Dequeue {Item}: {Operation}", item, op);
            return op;
        }
    }
}