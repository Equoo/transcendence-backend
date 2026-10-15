using Microsoft.AspNetCore.Authorization;


namespace KeepGrouped.API.Users.Invitations;

public static class InvitationEndpoints
{
    public static void MapInvitations(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/auth/invitation").WithTags("Invitations");

        group.MapCreateInvitation();
        group.MapListInvitation();
        group.MapDeleteInvitation();
    }
}