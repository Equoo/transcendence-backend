using KeepGrouped.API.Channels;
using KeepGrouped.API.Problems;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Conversations;

public sealed class DeleteConversationCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result> ExecuteAsync(string meId, string id)
	{
		Channel? conversation = await db.Channels
			.Include(c => c.Members)
			.SingleOrDefaultAsync(c =>
				c.Id == id &&
				(c.Type == ChannelType.DirectMessage || c.Type == ChannelType.GroupDirectMessage) &&
				c.Members.Any(m => m.UserId == meId));
		if (conversation is null)
		{
			return ConversationProblems.NotFound(id);
		}

		if (conversation.Type == ChannelType.DirectMessage)
		{
			return ConversationProblems.CannotLeaveDirectMessage();
		}

		ChannelMember membership = conversation.Members.Single(m => m.UserId == meId);
		conversation.Members.Remove(membership);
		db.ChannelMembers.Remove(membership);

		// The last member leaving takes the conversation with them.
		if (conversation.Members.Count == 0)
		{
			db.Channels.Remove(conversation);
		}

		await db.SaveChangesAsync();

		await hub.Clients.User(meId).SendAsync("RemoveConversation", conversation.Id);
		await hub.Clients.Users(conversation.Members.Select(m => m.UserId)).SendAsync("ConversationMemberLeft", conversation.Id, meId);

		return Result.OK;
	}
}
