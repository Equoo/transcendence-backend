namespace KeepGrouped.API.Problems;

static public class EventProblems
{
    static public IResult AlreadyRegistered(string username, string eventName)
    {
        return Results.Problem(
            title: "Already Registered",
            detail: $"User '{username}' is already registered to '{eventName}'.",
            statusCode: StatusCodes.Status409Conflict,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "ALREADY_REGISTERED" }
        );
    }
    static public IResult NotRegistered(string username, string eventName)
    {
        return Results.Problem(
            title: "Not Registered",
            detail: $"User '{username}' is not registered to '{eventName}'.",
            statusCode: StatusCodes.Status404NotFound,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "NOT_REGISTERED" }
        );
    }
    static public IResult EventFull()
    {
        return Results.Problem(
            title: "Event is full",
            detail: "The event has already reach the full capacity.",
            statusCode: StatusCodes.Status409Conflict,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "EVENT_FULL" }
        );
    }

    static public IResult EventRoleAlreadyExists(string name)
    {
        return Results.Problem(
            title: "Event role already exists",
            detail: $"An event role with this name: '{name}' already exists.",
            statusCode: StatusCodes.Status409Conflict,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "EVENTROLE_ALREADY_EXISTS" }
        );
    }
}
