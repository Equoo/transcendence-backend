using KeepGrouped.API.Channels;
using KeepGrouped.API.Problems;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Conversations;

public sealed class GetConversationQuery(KeepGroupedDb db) : IHandler
{
	public async Task<Result<ConversationResponse>> ExecuteAsync(string meId, string id)
	{
		Channel? conversation = await db.Channels
			.AsNoTracking()
			.Where(c =>
				c.Id == id &&
				(c.Type == ChannelType.DirectMessage || c.Type == ChannelType.GroupDirectMessage) &&
				c.Members.Any(m => m.UserId == meId))
			.Include(c => c.Members)
				.ThenInclude(m => m.User)
					.ThenInclude(u => u.Role)
			.SingleOrDefaultAsync();

		return conversation is null ? ConversationProblems.NotFound(id) : ConversationResponse.FromEntity(conversation, meId);
	}
}
