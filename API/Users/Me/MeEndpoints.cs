namespace KeepGrouped.API.Users;

public static class MeEndpoints
{
    public static void MapMe(this IEndpointRouteBuilder app)
    {
        var me = app.MapGroup("/me");

        me.MapGetMe();
        me.MapUpdateMe();
        me.MapDeleteMe();
        me.MapListMyTokens();
        me.MapPatchAvatar();
    }
}
