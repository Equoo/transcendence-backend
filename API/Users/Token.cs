using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KeepGrouped.API.Users;

public class RefreshToken
{
	public string Id { get; set; } = null!;
	public string UserId { get; set; } = null!;
	public DateTimeKind ExpireAt { get; set; }
}

public static class Token
{
	public static string BuildAcess(string id, HttpContext http)
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

		string s_token = new JwtSecurityTokenHandler().WriteToken(token);

		http.Response.Cookies.Append("AccessToken", s_token);

		return s_token;
	}

	public static string BuildRefresh(string id, HttpContext http)
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

		string s_token = new JwtSecurityTokenHandler().WriteToken(token);

		http.Response.Cookies.Append("RefreshToken", s_token);

		return s_token;
	}

	public static void AddTokenCookie(string token, string name, HttpContext http)
	{

		http.Response.Cookies.Append(name, token, new CookieOptions
		{
			HttpOnly = true,
			Secure = false
		});
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


	public static void RemoveCookies(HttpContext http)
	{
		http.Response.Cookies.Delete("AccessToken");
		http.Response.Cookies.Delete("RefreshToken");
	}

	public static string? GetCookie(HttpContext http, string key)
	{
		return http.Request.Cookies[key];
	}

	public static JwtSecurityToken GetToken(HttpContext http)
	{
		var cookie = http.Request.Cookies["AccessToken"];
		return new JwtSecurityTokenHandler().ReadJwtToken(cookie);
	}
}