using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class CategoryProblems
{
    static public ProblemHttpResult NotFound(string id) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "CATEGORY_NOT_FOUND",
        "Category not found",
        $"No category has the id '{id}'.");

    static public ProblemHttpResult NameAlreadyUsed(string name) => ProblemFactory.Create(
        StatusCodes.Status409Conflict, "CATEGORY_NAME_ALREADY_USED",
        "Category name already used",
        $"'{name}' is already used by another category.");

    static public ProblemHttpResult NameInvalid() => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "CATEGORY_NAME_INVALID",
        "Invalid category name",
        "Category name contains unauthorized characters.");
}
