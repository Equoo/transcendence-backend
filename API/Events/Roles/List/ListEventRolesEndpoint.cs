using Microsoft.AspNetCore.Authorization;

namespace KeepGrouped.API.Events;

public record ListEventRolesResponse(string Id, string Name)
{
    public static ListEventRolesResponse FromEntity(EventRole er) => new(er.Id, er.Name);
}

public static class ListEventRolesEndpoint
{
    public static void MapListEventRoles(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", [Authorize] async (ListEventRolesQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.Problem is { } problem)
            {
                return problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("eventroles.list")
        .WithSummary("List the event roles")
        .WithDescription("Returns every role that can be attached to an event. The implicit `Any` role is excluded.")
        .Produces<IEnumerable<ListEventRolesResponse>>(StatusCodes.Status200OK);
    }
}
