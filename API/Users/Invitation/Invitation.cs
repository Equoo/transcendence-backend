using System.Security.Cryptography;

namespace KeepGrouped.API.Users.Invitations;

public class Invitation : IExpirable, IIndentifier
{
    public static int Infinite { get; } = -1;
    public string Id { get; init; } = RandomNumberGenerator.GetHexString(32, true);
    public DateTime ExpireAt { get; set; }
    public int Usages { get; set; }
}