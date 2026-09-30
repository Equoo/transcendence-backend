
using System.Text.Json.Serialization;

namespace KeepGrouped.API.AiBackend.Delete;

public class DeleteResponse
{
	[JsonPropertyName("status")]
	public string Status { get; set; } = null!;

	[JsonPropertyName("document_id")]
	public string DocumentId { get; set; } = null!;

}