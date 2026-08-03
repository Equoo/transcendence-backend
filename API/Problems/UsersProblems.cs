namespace KeepGrouped.API.Problems;

static public class UserProblems
{
    static public IResult NameAlreadyUsed(string username)
    {
        return Results.Problem(
            title: "Username already used",
            detail: $"'{username}' is already used by an other user'.",
            statusCode: StatusCodes.Status409Conflict,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "NAME_ALREADY_USED" }
        );
    }

     static public IResult PasswordTooWeak()
    {
        return Results.Problem(
            title: "Password too weak",
            detail: "'Password choose is too weak'.",
            statusCode: StatusCodes.Status406NotAcceptable,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "WEAK_PASSWORD" }
        );
    }

    static public IResult AuthenticationInvalid()
    {
        return Results.Problem(
            title: "Invalid Authentication",
            detail: "'Username or Password is invalid'.",
            statusCode: StatusCodes.Status406NotAcceptable,
            extensions: new Dictionary<string, object?> { ["errorCode"] = "INVALID_AUTH" }
        );
    }
}