using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class RegistrationProblems
{
    static public ProblemHttpResult AlreadyRegistered(string username, string eventName) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "ALREADY_REGISTERED",
        "Already Registered",
        $"User '{username}' is already registered to '{eventName}'.");

    static public ProblemHttpResult NotRegistered(string username, string eventName) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "NOT_REGISTERED",
        "Not Registered",
        $"User '{username}' is not registered to '{eventName}'.");
}
