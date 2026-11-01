using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class RelationshipProblems
{
    static public ProblemHttpResult NotFound(string userId) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "RELATIONSHIP_NOT_FOUND",
        "Relationship not found",
        $"You have no relationship with the user '{userId}'.");

    static public ProblemHttpResult SelfRelationship() => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "SELF_RELATIONSHIP",
        "Self relationship",
        "You cannot have a relationship with yourself.");

    static public ProblemHttpResult AlreadyExists(string username) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "RELATIONSHIP_ALREADY_EXISTS",
        "Relationship already exists",
        $"You already have a friend request, a friendship or a block with '{username}'.");

    static public ProblemHttpResult NicknameEmpty() => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "RELATIONSHIP_NICKNAME_EMPTY",
        "Empty nickname",
        "The nickname cannot be empty or only whitespace. Send null to remove it.");
}
