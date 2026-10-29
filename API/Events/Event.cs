using KeepGrouped.API.Users;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Channels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Middlewares;
using KeepGrouped.API.Attributes.Roles;
using KeepGrouped.API.Rag;


namespace KeepGrouped.API.Events;

public class Event : IExpirable, IRagProcessable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    string IRagProcessable.RagId => Id;
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
    public DateTime ExpireAt { get; set; }
    public int Size { get; set; }
    public string Location { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = [];
    public string Description { get; set; } = string.Empty;
    public Channel Channel { get; set; } = null!;
    public User Organizer { get; set; } = null!;
    public ICollection<User> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public ICollection<EventRole> EventRoles { get; set; } = [];
    public ICollection<StorageFile> Files { get; set; } = [];
    public bool Ingested { get; set; } = false;
}

public record EventSummary(string Id,
    string Name,
    DateTime Date,
    int Size,
    string Location,
    ICollection<string> Tags,
    ICollection<EventRoleSummary> EventRoles,
    int RegisteredCount,
    bool IsRegistered
)
{
    public static EventSummary FromEntity(Event ev, bool isRegistered) => new(ev.Id, ev.Name,
        ev.Date, ev.Size, ev.Location, ev.Tags,
        [.. ev.EventRoles.Select(EventRoleSummary.FromEntity)],
        ev.Registrations.Count,
        isRegistered);
}
