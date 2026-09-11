using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KeepGrouped.API.Users.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeepGrouped.API.Users;

public class TokenProvider
{
    private readonly AuthenticationOptions _options;
    private readonly SymmetricSecurityKey _key;
    private readonly SigningCredentials _creds;

    public TokenProvider(IOptions<AuthenticationOptions> options)
    {
        _options = options.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.JWTKey));
        _creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);
    }

    public string CreateAccess(string id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
        };

        JwtSecurityToken token = new(
            issuer: "KeepGrouped",
            audience: "KeepGrouped",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: _creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefresh(string id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id),
        };

        JwtSecurityToken token = new(
            issuer: "KeepGrouped",
            audience: "KeepGrouped",
            claims: claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: _creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool IsValid(string token)
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
                IssuerSigningKey = _key
            };

            new JwtSecurityTokenHandler().ValidateToken(token, options, out SecurityToken validation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool CanRead(string token) => new JwtSecurityTokenHandler().CanReadToken(token);

    public string ReadFirstClaim(string token) =>
        new JwtSecurityTokenHandler().ReadJwtToken(token).Claims.First().Value;
}
