using System.Text.Json.Serialization;

namespace KeepGrouped.API.AiBackend.Ingest;

public class IngestResponse
{
	public string Status { get; set; } = null!;

	[JsonPropertyName("document_id")]
	public string DocumentId { get; set; } = null!;

	public List<string> Chunks { get; set; } = null!;
}