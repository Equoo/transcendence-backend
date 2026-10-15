using KeepGrouped.API.Channels;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Conversations;

public sealed class ListConversationsQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<ConversationResponse>>> ExecuteAsync(string meId, DateTime? before, int take)
	{
		var query = db.Channels
			.AsNoTracking()
			.Where(c =>
				(c.Type == ChannelType.DirectMessage || c.Type == ChannelType.GroupDirectMessage) &&
				c.Members.Any(m => m.UserId == meId));

		// Last activity = last message date, or creation date when there is no message
		if (before.HasValue)
		{
			query = query.Where(c => (c.LastMessage != null ? c.LastMessage.SentAt : c.CreateAt) < before.Value);
		}

		List<Channel> conversations = await query
			.OrderByDescending(c => c.LastMessage != null ? c.LastMessage.SentAt : c.CreateAt)
			.Take(take)
			.Include(c => c.Members)
				.ThenInclude(m => m.User)
					.ThenInclude(u => u.Role)
			.Include(c => c.LastMessage)
				.ThenInclude(m => m!.Sender)
			.ToListAsync();

		return conversations.Select(c => ConversationResponse.FromEntity(c, meId)).ToList();
	}
}
