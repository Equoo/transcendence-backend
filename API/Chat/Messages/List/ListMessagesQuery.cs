using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class ListMessagesQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<List<MessageResponse>>> ExecuteAsync(
        string channelId,
        DateTime? before,
        int take
    )
    {
        var channel = await db.Channels.SingleOrDefaultAsync(c => c.Id == channelId);
        if (channel is null)
        {
            return ChannelProblems.NotFound(channelId);
        }

        var query = db
            .Messages.Include(m => m.Sender)
            .Include(m => m.MessageRef)
                .ThenInclude(r => r!.Sender)
            .Include(m => m.Channel)
            .Where(m => m.ChannelId == channelId);

        if (before.HasValue)
        {
            query = query.Where(m => m.SentAt < before.Value);
        }

        var messages = await query
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .Select(m => MessageResponse.FromEntity(m))
            .ToListAsync();

        messages.Reverse();

        return messages;
    }
}
