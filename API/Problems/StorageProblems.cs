using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

static public class StorageProblems
{
    static public ProblemHttpResult FileNotFound(string key) => ProblemFactory.Create(
        StatusCodes.Status404NotFound, "FILE_NOT_FOUND",
        "File not found",
        $"No stored file has the key '{key}'.");

    static public ProblemHttpResult EmptyFile() => ProblemFactory.Create(
        StatusCodes.Status422UnprocessableEntity, "FILE_EMPTY",
        "Empty file",
        "The uploaded file has no content.");

    static public ProblemHttpResult UploadFailed(int status) => Upstream(
        "STORAGE_UPLOAD_FAILED", "Upload failed", "store the file", status);

    static public ProblemHttpResult DownloadFailed(int status) => Upstream(
        "STORAGE_DOWNLOAD_FAILED", "Download failed", "read the file back", status);

    static public ProblemHttpResult DeleteFailed(int status) => Upstream(
        "STORAGE_DELETE_FAILED", "Delete failed", "delete the file", status);

    /// <summary>
    /// The object store answered an error. It is reported as 502: the caller's request was fine,
    /// a dependency of this API failed — and its raw status is surfaced as <c>storageStatus</c>.
    /// </summary>
    static ProblemHttpResult Upstream(string errorCode, string title, string action, int status) =>
        TypedResults.Problem(
            title: title,
            detail: $"The object store refused to {action} and answered {status}.",
            statusCode: StatusCodes.Status502BadGateway,
            extensions: new Dictionary<string, object?>
            {
                ["errorCode"] = errorCode,
                ["storageStatus"] = status,
            }
        );
}
