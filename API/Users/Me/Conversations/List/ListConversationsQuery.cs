using KeepGrouped.API.Channels;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Conversations;

public sealed class ListConversationsQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<List<ConversationResponse>>> ExecuteAsync(string meId)
	{
		List<Channel> conversations = await db.Channels
			.AsNoTracking()
			.Where(c =>
				(c.Type == ChannelType.DirectMessage || c.Type == ChannelType.GroupDirectMessage) &&
				c.Members.Any(m => m.UserId == meId))
			.Include(c => c.Members)
				.ThenInclude(m => m.User)
					.ThenInclude(u => u.Role)
			.ToListAsync();

		return conversations.Select(c => ConversationResponse.FromEntity(c, meId)).ToList();
	}
}
