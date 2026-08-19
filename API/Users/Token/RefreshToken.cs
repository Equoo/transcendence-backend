namespace KeepGrouped.API.Users;

public class RefreshToken
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTimeKind ExpireAt { get; set; }
}
