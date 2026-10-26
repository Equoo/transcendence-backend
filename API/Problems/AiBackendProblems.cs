using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

public static class AiBackendProblems
{
	public static ProblemHttpResult ServiceUnavailable() =>
		TypedResults.Problem("AI is unavailable", statusCode: StatusCodes.Status503ServiceUnavailable);
	public static ProblemHttpResult EmptyFile() =>
		TypedResults.Problem("The uploaded file is empty.", statusCode: StatusCodes.Status400BadRequest);

	public static ProblemHttpResult UnsupportedFormat(string extension) =>
		TypedResults.Problem($"Unsupported format : {extension}", statusCode: StatusCodes.Status400BadRequest);

	public static ProblemHttpResult UpstreamUnavailable() =>
		TypedResults.Problem("The ingestion service is unavailable.", statusCode: StatusCodes.Status502BadGateway);
}