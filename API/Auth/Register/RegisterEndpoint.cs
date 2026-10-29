using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.Users;

public record RegisterRequest
{
	[Required]
	[Length(1, 255)]
	[RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Username can only contain letters, numbers, and underscores (_).")]

	public string Username { get; init; } = null!;
	[Required]
	[EmailAddress]
	[Length(3, 255)]
	public string Email { get; init; } = null!;
	[Required]
	[Length(8, 255)]
	public string Password { get; init; } = null!;
	[Required]
	public string InvitationCode { get; init; } = null!;
}

public record RegisterResponse(string Id, string UserName)
{
	public static RegisterResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class RegisterEndpoint
{
	public static void MapRegister(this IEndpointRouteBuilder auth)
	{
		auth.MapPost("/register", async (RegisterUserCommand command, RegisterRequest req, HttpContext http) =>
		{
			var result = await command.ExecuteAsync(req);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			TokenCookies.Write(http, result.Value.Tokens);

			return Results.CreatedAtRoute("users.get", new { id = result.Value.User.Id }, result.Value.User);
		})
		.WithName("auth.register")
		.WithSummary("Register a new user")
		.WithDescription("Creates a user and logs them in: the response sets the `AccessToken` and `RefreshToken` cookies.")
		.Produces<RegisterResponse>(StatusCodes.Status201Created)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status409Conflict);
	}
}
