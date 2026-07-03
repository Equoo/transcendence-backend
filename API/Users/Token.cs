using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.SignalR;
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

    public static UserResponse GetUserRespByToken(JwtSecurityToken token)
    {
       UserResponse resp = new();

       foreach (Claim claim in token.Claims)
        {
            if (claim.Type == ClaimTypes.Name)
                resp.UserName = claim.Value;
            if (claim.Type == ClaimTypes.NameIdentifier)
                resp.Id = claim.Value;
        }

        return resp;
    }

    public static void RemoveCookie(HttpContext http, string key)
    {
        http.Response.Cookies.Delete(key);
    }

    public static string? GetCookie(HttpContext http, string key)
    {
        return http.Request.Cookies[key];
    }

    public static JwtSecurityToken GetToken(HttpContext http)
    {
        var cookie = http.Request.Cookies["AuthToken"];
        return new JwtSecurityTokenHandler().ReadJwtToken(cookie);
    }
}