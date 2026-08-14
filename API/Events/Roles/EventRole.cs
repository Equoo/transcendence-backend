namespace KeepGrouped.API.Events;

public class EventRole
{
    public const string Implicit = "Any";

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;

    public ICollection<Event> Events { get; } = [];
}

public record EventRoleSummary(string Id, string Name)
{
    public static EventRoleSummary FromEntity(EventRole er) => new(er.Id, er.Name);
}
