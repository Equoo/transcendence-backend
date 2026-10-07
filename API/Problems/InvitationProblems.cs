using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class InvitationProblems
{
    static public ProblemHttpResult InvalidInvitation() => ProblemFactory.Create(
        StatusCodes.Status406NotAcceptable, "INVALID_INVITATION",
        "Invalid Invitation",
        "You need a valid invitation with enough remaining uses to register.");

    static public ProblemHttpResult ExpiredInvitation(DateTime date) => ProblemFactory.Create(
        StatusCodes.Status406NotAcceptable, "EXPIRED_INVITATION",
        "Invitation Expired",
        $"Invitation expired on {date}."
    );

    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "INVITATION_NOT_FOUND",
        "Invitation not found",
        $"No invitation has the id '{id}'.");
}
