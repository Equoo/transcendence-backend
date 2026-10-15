using KeepGrouped.API.Channels;
using KeepGrouped.API.Problems;
using KeepGrouped.API.Users.Me.Relationships;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Conversations;

public sealed class CreateConversationCommand(KeepGroupedDb db, IHubContext<KeepGroupedHub> hub) : IHandler
{
	public async Task<Result<ConversationResponse>> ExecuteAsync(User me, CreateConversationRequest req)
	{
		List<string> recipientIds = [.. req.Recipients.Distinct()];
		if (recipientIds.Contains(me.Id))
		{
			return ConversationProblems.SelfRecipient();
		}

		List<User> recipients = await db.Users
			.Include(u => u.Role)
			.Where(u => recipientIds.Contains(u.Id))
			.ToListAsync();

		string? missing = recipientIds.FirstOrDefault(id => recipients.All(u => u.Id != id));
		if (missing is not null)
		{
			return UserProblems.NotFound(missing);
		}

		Relationship? block = await db.Relationships
			.Include(r => r.Me)
			.Include(r => r.User)
			.FirstOrDefaultAsync(r => r.Type == RelationshipState.Blocked &&
				((r.MeId == me.Id && recipientIds.Contains(r.UserId)) ||
				 (r.UserId == me.Id && recipientIds.Contains(r.MeId))));
		if (block is not null)
		{
			return ConversationProblems.Blocked(block.MeId == me.Id ? block.User.UserName : block.Me.UserName);
		}

		var type = recipientIds.Count == 1 ? ChannelType.DirectMessage : ChannelType.GroupDirectMessage;

		if (type == ChannelType.DirectMessage)
		{
			string recipientId = recipientIds[0];
			Channel? existing = await db.Channels
				.Where(c =>
					c.Type == ChannelType.DirectMessage &&
					c.Members.Any(m => m.UserId == me.Id) &&
					c.Members.Any(m => m.UserId == recipientId))
				.Include(c => c.Members)
					.ThenInclude(m => m.User)
						.ThenInclude(u => u.Role)
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (existing is not null)
			{
				return ConversationResponse.FromEntity(existing, me.Id);
			}
		}

		var conversation = new Channel(type == ChannelType.GroupDirectMessage ? req.Name ?? string.Empty : string.Empty, string.Empty, null)
		{
			Type = type,
			CategorySync = false
		};

		foreach (User user in recipients.Append(me))
		{
			conversation.Members.Add(new ChannelMember { ChannelId = conversation.Id, Channel = conversation, UserId = user.Id, User = user });
		}

		db.Channels.Add(conversation);
		await db.SaveChangesAsync();

		foreach (User user in recipients.Append(me))
		{
			await hub.Clients.User(user.Id).SendAsync("NewConversation", ConversationResponse.FromEntity(conversation, user.Id));
		}

		return ConversationResponse.FromEntity(conversation, me.Id);
	}
}
