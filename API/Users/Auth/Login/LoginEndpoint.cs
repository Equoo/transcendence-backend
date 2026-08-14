using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.Users;

public record LoginRequest
{
    [Required]
    public string UserName { get; init; } = null!;
    [Required]
    public string Password { get; init; } = null!;
}

/// <summary>
/// Who was just logged in. The tokens are not in the body on purpose: they travel as cookies.
/// </summary>
public record LoginResponse(string Id, string UserName)
{
    public static LoginResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class LoginEndpoint
{
    public static void MapLogin(this IEndpointRouteBuilder auth)
    {
        auth.MapPost("/login", async (LoginUserCommand command, LoginRequest req, HttpContext http) =>
        {
            var result = await command.ExecuteAsync(req);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            TokenCookies.Write(http, result.Value.Tokens);

            return Results.Ok(result.Value.User);
        })
        .WithName("auth.login")
        .WithSummary("Log a user in")
        .WithDescription("Verifies the credentials and sets the `AccessToken` and `RefreshToken` cookies.")
        .Produces<LoginResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status406NotAcceptable);
    }
}
