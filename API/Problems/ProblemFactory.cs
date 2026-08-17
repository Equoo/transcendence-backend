using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static class ProblemFactory
{
    public static ProblemHttpResult Create(int status, string errorCode, string title, string detail) =>
        TypedResults.Problem(
            title: title,
            detail: detail,
            statusCode: status,
            extensions: new Dictionary<string, object?> { ["errorCode"] = errorCode }
        );

    public static string Join(IEnumerable<string> values) => string.Join("', '", values);
}
