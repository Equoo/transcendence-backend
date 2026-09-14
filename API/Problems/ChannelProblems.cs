using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class ChannelProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "CHANNEL_NOT_FOUND",
        "Channel not found",
        $"No channel has the id '{id}'.");

    static public ProblemHttpResult NameAlreadyUsed(string name) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "CHANNEL_NAME_ALREADY_USED",
        "Channel name already used",
        $"'{name}' is already used by another channel.");

    static public ProblemHttpResult NameInvalid() => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "CHANNEL_NAME_INVALID",
        "Invalid channel name",
        "Channel name contains unauthorized characters.");
}
