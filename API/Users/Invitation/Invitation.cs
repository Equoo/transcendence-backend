using System.Security.Cryptography;

namespace KeepGrouped.API.Users.Invitation;

public class Invitation
{
    public string Id { get; init; } = RandomNumberGenerator.GetHexString(32, true);
    public DateTime ExpiresAt { get; init; }
    public int Usages { get; init; }
}