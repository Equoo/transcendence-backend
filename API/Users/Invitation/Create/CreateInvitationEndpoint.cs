using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Users.Invitation;

public record CreateInvitationRequest
{
    [Required]
    public DateTime ExpiresAt { get; init; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Usages { get; init; }
}

public static class CreateInvitationEndpoint
{
    public static void MapCreateInvitation(this RouteGroupBuilder route)
    {
        route.MapPost("/", [Authorize] async (CreateInvitationCommand cmd, CreateInvitationRequest req) =>
        {
            var result = await cmd.ExecuteAsync(req);

            if (result.Problem is { } problem)
            {
                return problem;
            }
            return Results.Ok(result.Value);
        })
        .WithName("invitations.create")
        .WithSummary("Create an invatation")
        .WithDescription("Creates an invitation with and id that will serve as the invitation link with a limited amount of uses, infinite uses means uint max")
        .Produces<string>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError); ;
    }
}