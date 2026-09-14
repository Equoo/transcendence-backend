using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

public static class MessageProblems
{
    public static ProblemHttpResult NotFound(string id) =>
        ProblemFactory.Create(
            StatusCodes.Status404NotFound,
            "MESSAGE_NOT_FOUND",
            "Message not found",
            $"No message has the id '{id}'."
        );

    public static ProblemHttpResult NotSender() =>
        ProblemFactory.Create(
            StatusCodes.Status401Unauthorized,
            "MESSAGE_NOT_SENDER",
            "Not the sender",
            "This operation can only be performed by the message's sender."
        );

    public static ProblemHttpResult TooLong() =>
        ProblemFactory.Create(
            StatusCodes.Status404NotFound,
            "MESSAGE_TOO_LONG",
            "Message too long",
            $"Message content has more than 8192 characters."
        );
}
