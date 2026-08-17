namespace KeepGrouped.API.Chat;

public class Channel
{
    public Channel() { }

    public Channel(string name) { }

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public ICollection<string> Messages { get; } = [];
}
