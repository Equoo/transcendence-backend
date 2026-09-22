using System.ComponentModel.DataAnnotations;

namespace KeepGrouped.API.AiBackend;

public record IngestRequest
{
	[Required]
	public IFormFile File { get; init; } = null!;
}