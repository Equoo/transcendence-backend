using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class UserProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "USER_NOT_FOUND",
        "User not found",
        $"No user has the id '{id}'.");

    static public ProblemHttpResult NameAlreadyUsed(string username) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "NAME_ALREADY_USED",
        "Username already used",
        $"'{username}' is already used by an other user.");

    static public ProblemHttpResult NotAuthenticated() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "NOT_AUTHENTICATED",
        "Not authenticated",
        "This operation acts on behalf of the current user, but no user is authenticated.");
};
