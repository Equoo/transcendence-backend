using KeepGrouped.API.Problems;
using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Chat;

public sealed class UpdateMessageCommand(KeepGroupedDb db) : IHandler
{
    public async Task<Result> ExecuteAsync(string msgId, User? sender, UpdateMessageRequest req)
    {
        if (sender is null)
        {
            return UserProblems.NotAuthenticated();
        }

        var msg = await db.Messages.SingleOrDefaultAsync(m => m.Id == msgId);
        if (msg is null)
        {
            return MessageProblems.NotFound(msgId);
        }

        if (msg.SenderId != sender.Id)
        {
            return MessageProblems.NotSender();
        }

        msg.Content = req.Content;
        msg.EditAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Result.OK;
    }
}
