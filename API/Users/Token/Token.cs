using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace KeepGrouped.API.Users;

public static class Token
{
    public static string CreateAccess(string id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken token = new(
            issuer: "KeepGrouped",
            audience: "KeepGrouped",
            claims: claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string CreateRefresh(string id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        JwtSecurityToken token = new(
            issuer: "KeepGrouped",
            audience: "KeepGrouped",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static bool IsValid(string token)
    {
        try
        {
            TokenValidationParameters options = new()
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = "KeepGrouped",
                ValidAudience = "KeepGrouped",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"))
            };

            new JwtSecurityTokenHandler().ValidateToken(token, options, out SecurityToken validation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool CanRead(string token) => new JwtSecurityTokenHandler().CanReadToken(token);

    /// <summary>Reads the first claim of a token without validating it — that is the user or token id.</summary>
    public static string ReadFirstClaim(string token) =>
        new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.First().Value;
}
