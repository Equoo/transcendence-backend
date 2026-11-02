using System.ComponentModel.DataAnnotations;
using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users;

public record UpdateMeEmailRequest
{
	[Required]
	[EmailAddress]
	[Length(3, 255)]
	public string Email { get; init; } = null!;

	[Required]
	public string Password { get; init; } = null!;
}

public record UpdateMeEmailResponse(string Id, string Email)
{
	public static UpdateMeEmailResponse FromEntity(User user) => new(user.Id, user.Email);
}

public static class UpdateMeEmailEndpoint
{
	public static void MapUpdateMeEmail(this IEndpointRouteBuilder me)
	{
		me.MapPatch("/email", [Authorize] async (UpdateMeEmailCommand command, UpdateMeEmailRequest req, TokenContext tk) =>
		{
			var result = await command.ExecuteAsync(tk.User, req);
			if (result.IsProblem)
			{
				return result.Problem;
			}

			return Results.Ok(result.Value);
		})
		.WithName("me.email")
		.WithSummary("Change the current user's email")
		.WithDescription("Changes the current user's email after checking their password. Emails are unique across the API.")
		.Produces<UpdateMeEmailResponse>(StatusCodes.Status200OK)
		.ProducesValidationProblem()
		.ProducesProblem(StatusCodes.Status401Unauthorized)
		.ProducesProblem(StatusCodes.Status403Forbidden)
		.ProducesProblem(StatusCodes.Status406NotAcceptable)
		.ProducesProblem(StatusCodes.Status409Conflict);
	}
}
