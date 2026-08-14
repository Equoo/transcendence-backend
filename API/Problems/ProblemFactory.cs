using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

/// <summary>
/// Builds the <c>ProblemDetails</c> responses of the API. Every problem carries an <c>errorCode</c>
/// extension: that string is the stable contract API consumers branch on, the HTTP status alone is not.
/// </summary>
static class ProblemFactory
{
    public static ProblemHttpResult Create(int status, string errorCode, string title, string detail) =>
        TypedResults.Problem(
            title: title,
            detail: detail,
            statusCode: status,
            extensions: new Dictionary<string, object?> { ["errorCode"] = errorCode }
        );

    /// <summary>Formats a set of offending identifiers for a problem detail, keeping it readable.</summary>
    public static string Join(IEnumerable<string> values) => string.Join("', '", values);
}
