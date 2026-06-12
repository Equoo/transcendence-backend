using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeepGrouped.API.Events;

class EvenementConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
    }
}

[EntityTypeConfiguration(typeof(EvenementConfiguration))]
public class Event
{
    public Event()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Id { get; set; }
    required public string Name { get; set; }
    required public DateTime Date { get; set; }
    required public int Size { get; set; }
    public string? Description { get; set; }

    public ICollection<ApplicationUser> Users { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}