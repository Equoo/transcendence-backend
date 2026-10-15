using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.AiBackend.Ingest;

public record CreateRagRequest
{
	[Required]
	public IFormFile File { get; init; } = null!;
}