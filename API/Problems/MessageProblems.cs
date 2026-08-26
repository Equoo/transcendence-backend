using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class MessageProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "MESSAGE_NOT_FOUND",
        "Message not found",
        $"No message has the id '{id}'.");

    static public ProblemHttpResult NotSender() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "MESSAGE_NOT_SENDER",
        "Not the sender",
        "This operation can only be performed by the message's sender.");
}
