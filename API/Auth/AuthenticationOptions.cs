using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.Users.Auth;

public class AuthenticationOptions
{
    public const string SectionName = "Authentication";

    [Required]
    public required string DefaultAdminLogin { get; init; }

    [Required]
    public required string DefaultAdminPwd { get; init; }

    [Required]
    public required string JWTKey { get; init; }
}