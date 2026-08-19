using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class RoleProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "ROLE_NOT_FOUND",
        "Role not found",
        $"No role has the id '{id}'.");
}
