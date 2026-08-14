using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record ListRegistrationsResponse(UserSummary User, DateTime RegisteredAt, string? Role)
{
    public static ListRegistrationsResponse FromEntity(Registration reg) => new(
        UserSummary.FromEntity(reg.User),
        reg.RegisteredAt,
        reg.Role?.Name);
}

public static class ListRegistrationsEndpoint
{
    public static void MapListRegistrations(this IEndpointRouteBuilder registrations)
    {
        registrations.MapGet("/", [Authorize] async (ListRegistrationsQuery query, string id) =>
        {
            var result = await query.ExecuteAsync(id);
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("registrations.list")
        .WithSummary("List the registrations of an event")
        .WithDescription("Returns every user registered to the event, with their role and registration date.")
        .Produces<IEnumerable<ListRegistrationsResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
