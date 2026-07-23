namespace KeepGrouped.API;

using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
using KeepGrouped.API.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options)
    : IdentityDbContext<User>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder
            .Entity<Event>()
            .HasMany(e => e.Users)
            .WithMany(e => e.Events)
            .UsingEntity<Registration>();
        builder.Entity<Event>().HasOne(e => e.Organizer);
        builder.Entity<EventRole>().HasAlternateKey(er => er.Name);

        builder.Entity<Event>().Navigation(e => e.Organizer).AutoInclude();
        builder.Entity<Event>().Navigation(e => e.Registrations).AutoInclude();
        builder.Entity<Event>().Navigation(e => e.EventRoles).AutoInclude();
        builder.Entity<Registration>().Navigation(e => e.User).AutoInclude();
    }

    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventRole> EventRoles { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
}
