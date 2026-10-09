using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class ConversationProblems
{
	static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
		StatusCodes.Status404NotFound, "CONVERSATION_NOT_FOUND",
		"Conversation not found",
		$"You have no conversation with the id '{id}'.");

	static public ProblemHttpResult SelfRecipient() => ProblemFactory.Create(
		StatusCodes.Status422UnprocessableEntity, "CONVERSATION_SELF_RECIPIENT",
		"Self recipient",
		"You cannot be a recipient of your own conversation.");

	static public ProblemHttpResult Blocked(string username) => ProblemFactory.Create(
		StatusCodes.Status403Forbidden, "CONVERSATION_BLOCKED",
		"Conversation blocked",
		$"You cannot start a conversation with '{username}'.");

	static public ProblemHttpResult CannotLeaveDirectMessage() => ProblemFactory.Create(
		StatusCodes.Status422UnprocessableEntity, "CONVERSATION_DIRECT_MESSAGE_LEAVE",
		"Cannot leave a direct message",
		"Only group conversations can be left.");
}
