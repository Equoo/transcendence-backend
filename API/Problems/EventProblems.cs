using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class EventProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "EVENT_NOT_FOUND",
        "Event not found",
        $"No event has the id '{id}'.");

    static public ProblemHttpResult Full(string eventName, int size) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "EVENT_FULL",
        "Event is full",
        $"'{eventName}' has already reached its capacity of {size} participants.");

    static public ProblemHttpResult UnknownEventRoles(IEnumerable<string> ids) => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "EVENT_ROLES_UNKNOWN",
        "Unknown event roles",
        $"No event role has the id '{ProblemFactory.Join(ids)}'.");

    static public ProblemHttpResult UnknownFiles(IEnumerable<string> keys) => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "EVENT_FILES_UNKNOWN",
        "Unknown files",
        $"No stored file has the key '{ProblemFactory.Join(keys)}'.");

    static public ProblemHttpResult SizeBelowRegistrations(int size, int registered) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "EVENT_SIZE_TOO_SMALL",
        "Event size below its registrations",
        $"The event already has {registered} participants, its size cannot be lowered to {size}.");

    /// <summary>
    /// The implicit `Any` role is expected to be seeded; without it an event cannot be created.
    /// This is a server-side inconsistency, not a caller mistake.
    /// </summary>
    static public ProblemHttpResult DefaultRoleMissing() => ProblemFactory.Create(
        StatusCodes.Status500InternalServerError, "EVENT_DEFAULT_ROLE_MISSING",
        "Default event role missing",
        "The implicit 'Any' event role is absent from the database, events cannot be created.");
}
