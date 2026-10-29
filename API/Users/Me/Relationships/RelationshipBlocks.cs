using KeepGrouped.API.Channels;
using Microsoft.EntityFrameworkCore;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class RelationshipBlocks
{
	public static Task<bool> IsBlockedBetweenAsync(this KeepGroupedDb db, string userId, string otherId) =>
		db.Relationships.AnyAsync(r =>
			r.Type == RelationshipState.Blocked &&
			((r.MeId == userId && r.UserId == otherId) || (r.MeId == otherId && r.UserId == userId)));

	public static async Task<bool> IsDirectMessageBlockedAsync(this KeepGroupedDb db, Channel channel, string senderId) =>
		channel.Type == ChannelType.DirectMessage &&
		channel.Members.FirstOrDefault(m => m.UserId != senderId) is { } recipient &&
		await db.IsBlockedBetweenAsync(senderId, recipient.UserId);
}
