namespace KeepGrouped.API.Users;

public static class UserEndpoint
{
    public static void MapUsers(this IEndpointRouteBuilder app)
    {
        var users = app.MapGroup("/users").WithTags("Users");

        users.MapListUsers();
        users.MapGetUser();
    }
}
