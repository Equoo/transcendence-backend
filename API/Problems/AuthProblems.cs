using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class AuthProblems
{
    static public ProblemHttpResult InvalidCredentials() => ProblemFactory.Create(
        StatusCodes.Status406NotAcceptable, "INVALID_AUTH",
        "Invalid Authentication",
        "Username or Password is invalid.");

    static public ProblemHttpResult RefreshTokenMissing() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "REFRESH_TOKEN_MISSING",
        "Refresh token missing",
        "The request carries no RefreshToken cookie.");

    static public ProblemHttpResult RefreshTokenInvalid() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "REFRESH_TOKEN_INVALID",
        "Refresh token invalid",
        "The RefreshToken cookie is malformed, expired or was not issued by this server.");

    static public ProblemHttpResult RefreshTokenUnknown() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "REFRESH_TOKEN_UNKNOWN",
        "Refresh token unknown",
        "The refresh token is well-formed but is not stored server-side: it was already rotated or revoked.");
}
