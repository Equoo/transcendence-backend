using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace KeepGrouped.API.Users;

public static class Token
{
    public static string Build(string UserName, string Id)
        {
            var claims = new[]
                {
                    new Claim(ClaimTypes.Name, UserName),
                    new Claim(ClaimTypes.NameIdentifier, Id),
                };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CLE-DUR-COMME-DE-LA-PIERRE-MAINTENANT-BIEN-PLUS-RESISTANTE-PARCEQUECAMARCHAITPASAVANT"));
                var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

                var token = new JwtSecurityToken(
                    issuer: "KeepGrouped",
                    audience: "KeepGrouped",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    public static void AddToCookie(string token, HttpContext http)
    {
            
        http.Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false
        });
    }
}