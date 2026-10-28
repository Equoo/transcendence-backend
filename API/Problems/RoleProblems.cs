using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class RoleProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "ROLE_NOT_FOUND",
        "Role not found",
        $"No role has the id '{id}'.");

    static public ProblemHttpResult NameAlreadyUsed(string name) => ProblemFactory.Create(
    StatusCodes.Status409Conflict, "NAME_ALREADY_USED",
    "Role Name already used",
    $"'{name}' is already used by an other role.");

    static public ProblemHttpResult NotFound() => ProblemFactory.Create(
   StatusCodes.Status404NotFound, "ROLE_NOT_FOUND",
   "Role not found",
   $"No role has the id.");

    static public ProblemHttpResult RankingTooLow() => ProblemFactory.Create(
  StatusCodes.Status404NotFound, "RANKING_TOO_LOW",
  "Ranking too low", "Your actual role ranking is too low to doing this action");
}
