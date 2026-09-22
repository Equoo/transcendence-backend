using Microsoft.AspNetCore.Http.HttpResults;

namespace KeepGrouped.API.Problems;

public static class AiBackendProblems
{
	public static ProblemHttpResult EmptyFile() =>
		TypedResults.Problem("Le fichier envoyé est vide.", statusCode: StatusCodes.Status400BadRequest);

	public static ProblemHttpResult UnsupportedFormat(string extension) =>
		TypedResults.Problem($"Format non supporté : {extension}", statusCode: StatusCodes.Status400BadRequest);

	public static ProblemHttpResult UpstreamUnavailable() =>
		TypedResults.Problem("Le service d'ingestion est indisponible.", statusCode: StatusCodes.Status502BadGateway);
}