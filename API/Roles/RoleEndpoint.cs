
namespace KeepGrouped.API.Roles;

public static class RolesEndpoints
{
    public static void MapRoles(this IEndpointRouteBuilder app)
    {
        var roles = app.MapGroup("/roles").WithTags("Roles");

        roles.MapGetRoles();
        roles.MapCreateRole();
        roles.MapPatchPermRole();
        roles.MapPatchNameRole();
        roles.MapDeleteRole();
    }
}