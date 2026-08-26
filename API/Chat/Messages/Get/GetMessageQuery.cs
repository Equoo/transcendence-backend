using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class GetMessageQuery(KeepGroupedDb db) : IHandler
{
    public async Task<Result<MessageResponse>> ExecuteAsync(string msgId)
    {
        var msg = await db.Messages.AsNoTracking().Include(m => m.Sender).Include(m => m.Channel)
            .SingleOrDefaultAsync(m => m.Id == msgId);

        return msg is null ? MessageProblems.NotFound(msgId) : MessageResponse.FromEntity(msg);
    }
}
