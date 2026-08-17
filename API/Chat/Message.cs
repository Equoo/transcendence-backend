namespace KeepGrouped.API.Chat;

public class Message
{
    public Message() { }

    public Message(string name) { }

    public string text { get; } = null!;
    public string owner { get; } = null!;
    public DateTime date { get; } = DateTime.Now;
}
