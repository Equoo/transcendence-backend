using KeepGrouped.API.Middlewares;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public static class DeleteRegistrationEndpoint
{
    public static void MapDeleteRegistration(this IEndpointRouteBuilder registrations)
    {
        registrations.MapDelete("/", [Authorize] async (DeleteRegistrationCommand command, string id, TokenContext token) =>
        {
            var result = await command.ExecuteAsync(id, token.User);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.NoContent();
        })
        .WithName("registrations.delete")
        .WithSummary("Cancel a registration")
        .WithDescription("Removes the current user's registration from the event.")
        .Produces(StatusCodes.Status204NoContent)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
