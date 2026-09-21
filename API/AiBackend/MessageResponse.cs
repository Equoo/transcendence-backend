
public class MessageResponse
{
	public string Id { get; set; } = null!;
	public string ChannelId { get; set; } = null!;
	public string Content { get; set; } = null!;
	public string? MessageReferenceId { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}