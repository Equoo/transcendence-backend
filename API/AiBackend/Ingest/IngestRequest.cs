using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.AiBackend.Ingest;

public record IngestRequest
{
	[Required]
	public IFormFile File { get; init; } = null!;
}