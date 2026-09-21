using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class MeProblems
{
    static public ProblemHttpResult InvalidPassword() => ProblemFactory.Create
    (StatusCodes.Status406NotAcceptable, "INVALID_AUTH",
        "Invalid Authentication",
        "Password is invalid.");
}