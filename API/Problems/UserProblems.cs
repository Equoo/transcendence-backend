using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class UserProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "USER_NOT_FOUND",
        "User not found",
        $"No user has the id '{id}'.");

    static public ProblemHttpResult NameAlreadyUsed(string username) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "NAME_ALREADY_USED",
        "Username already used",
        $"'{username}' is already used by an other user.");

    static public ProblemHttpResult NotAuthenticated() => ProblemFactory.Create(
        StatusCodes.Status401Unauthorized, "NOT_AUTHENTICATED",
        "Not authenticated",
        "This operation acts on behalf of the current user, but no user is authenticated.");

    static public ProblemHttpResult InvalidAvatarType(string contentType) => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "INVALID_AVATAR_TYPE",
        "Invalid avatar type",
        $"'{contentType}' is not an image. The avatar must be an image file.");

    static public ProblemHttpResult UserCannotBeDelete() => ProblemFactory.Create(
   StatusCodes.Status401Unauthorized, "USER_CANNOT_BE_DELETED",
   "User cannot be deleted",
   "This user cannot be deleted");

    static public ProblemHttpResult PasswordCannotBeChanged() => ProblemFactory.Create(
  StatusCodes.Status401Unauthorized, "PASSWORD_CANNOT_BE_CHANGED",
  "Password cannot be changed",
  "This user's password cannot be changed");

    static public ProblemHttpResult UsernameCannotBeChanged() => ProblemFactory.Create(
 StatusCodes.Status401Unauthorized, "USERNAME_CANNOT_BE_CHANGED",
 "Username cannot be changed",
 "This user's name cannot be changed");

};
