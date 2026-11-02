using KeepGrouped.API.Channels;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class RelationshipBlocks
{
	public static async Task<bool> IsDirectMessageBlockedAsync(this KeepGroupedDb db, Channel channel, string senderId) =>
		channel.Type == ChannelType.DirectMessage &&
		channel.Members.FirstOrDefault(m => m.UserId != senderId) is { } recipient &&
		await db.Relationships.AnyAsync(r =>
			r.Type == RelationshipState.Blocked && r.MeId == senderId && r.UserId == recipient.UserId);

	public static async Task<List<string>> ConversationBlockersOfAsync(this KeepGroupedDb db, Channel channel, string senderId) =>
		channel.IsConversation
			? await db.Relationships
				.Where(r => r.Type == RelationshipState.Blocked && r.UserId == senderId)
				.Select(r => r.MeId)
				.ToListAsync()
			: [];
}
