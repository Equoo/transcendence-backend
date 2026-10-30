namespace KeepGrouped.API.Users;

public class RefreshToken : IExpirable
{
    public string Id { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime ExpireAt { get; set; }
}
