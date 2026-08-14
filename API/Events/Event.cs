using KeepGrouped.API.Users;
using KeepGrouped.API.Storage;

namespace KeepGrouped.API.Events;

public class Event
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = null!;
    public DateTime Date { get; set; }
    public int Size { get; set; }
    public string Location { get; set; } = null!;
    public ICollection<string> Tags { get; set; } = [];
    public string Description { get; set; } = string.Empty;

    public User Organizer { get; set; } = null!;
    public ICollection<User> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
    public ICollection<EventRole> EventRoles { get; init; } = [];
    public ICollection<StorageFile> Files { get; init; } = [];
}
