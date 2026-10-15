
namespace KeepGrouped.API.Users.Me.Relationships;

public enum RelationshipState
{
	PendingRequest,
	PendingResponse,
	Friend,
	Blocked,
}

public class Relationship
{
	public string Id { get; init; } = Guid.NewGuid().ToString();
	public string MeId { get; init; } = null!;
	public User Me { get; init; } = null!;
	public string UserId { get; init; } = null!;
	public User User { get; init; } = null!;
	public string? Nickname { get; set; } = null;
	public RelationshipState Type { get; set; } = RelationshipState.PendingRequest;
	public DateTime Since { get; init; } = DateTime.UtcNow;
}

public record RelationshipResponse(string Id, UserSummary User, string? Nickname, RelationshipState Type, DateTime Since)
{
	public static RelationshipResponse FromEntity(Relationship relationship) => new(relationship.Id,
		UserSummary.FromEntity(relationship.User), relationship.Nickname, relationship.Type, relationship.Since);
}
