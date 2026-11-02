using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace KeepGrouped.API.Users.Me.Relationships;

public static class RelationshipLocks
{
	public static async Task<IDbContextTransaction> LockPairAsync(this KeepGroupedDb db, string userId, string otherId)
	{
		string key = string.CompareOrdinal(userId, otherId) < 0 ? $"{userId}:{otherId}" : $"{otherId}:{userId}";
		var tx = await db.Database.BeginTransactionAsync();
		await db.Database.ExecuteSqlAsync($"SELECT pg_advisory_xact_lock(hashtext({key}))");
		return tx;
	}
}
