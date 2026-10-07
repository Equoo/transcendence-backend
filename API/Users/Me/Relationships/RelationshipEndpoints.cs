namespace KeepGrouped.API.Users.Me.Relationships;

public static class RelationshipEndpoints
{
    public static void MapRelationships(this IEndpointRouteBuilder me)
    {
        var relationships = me.MapGroup("/relationships").WithTags("Relationships");

        relationships.MapListRelationships();
        relationships.MapCreateRelationship();
        relationships.MapBlockRelationship();
        relationships.MapUpdateRelationship();
        relationships.MapDeleteRelationship();
    }
}
