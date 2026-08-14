using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class EventRoleProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "EVENTROLE_NOT_FOUND",
        "Event role not found",
        $"No event role has the id '{id}'.");

    static public ProblemHttpResult AlreadyExists(string name) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "EVENTROLE_ALREADY_EXISTS",
        "Event role already exists",
        $"An event role with this name: '{name}' already exists.");

    /// <summary>`Any` is attached to every event implicitly; it cannot be created or requested by name.</summary>
    static public ProblemHttpResult Reserved(string name) => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "EVENTROLE_RESERVED",
        "Reserved event role",
        $"'{name}' is a reserved role managed by the server and cannot be created.");

    static public ProblemHttpResult NotOfferedByEvent(string roleName, string eventName) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "EVENTROLE_NOT_OFFERED",
        "Event role not offered",
        $"'{eventName}' does not offer the role '{roleName}'.");
}
