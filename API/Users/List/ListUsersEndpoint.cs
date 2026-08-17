namespace KeepGrouped.API.Users;

public record ListUsersResponse(string Id, string UserName)
{
    public static ListUsersResponse FromEntity(User user) => new(user.Id, user.UserName);
}

public static class ListUsersEndpoint
{
    public static void MapListUsers(this IEndpointRouteBuilder users)
    {
        users.MapGet("/", async (ListUsersQuery query) =>
        {
            var result = await query.ExecuteAsync();
            if (result.IsProblem)
            {
                return result.Problem;
            }

            return Results.Ok(result.Value);
        })
        .WithName("users.list")
        .WithSummary("List users")
        .WithDescription("Returns the public profile of every registered user.")
        .Produces<IEnumerable<ListUsersResponse>>(StatusCodes.Status200OK);
    }
}
