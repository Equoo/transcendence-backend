using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class TokensProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "TOKEN_NOT_FOUND",
        "Token not found",
        $"No token has the user id '{id}'.");
    
}
