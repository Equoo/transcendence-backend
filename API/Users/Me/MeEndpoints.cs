namespace KeepGrouped.API.Users;

public static class MeEndpoints
{
    public static void MapMe(this IEndpointRouteBuilder app)
    {
        var me = app.MapGroup("/me");

        me.MapGetMe();
        me.MapDeleteMe();
        me.MapDeleteAvatar();
        me.MapPatchAvatar();
        me.MapUpdateMe();
        me.MapUpdateMePass();
        me.MapListMyTokens();
    }
}
