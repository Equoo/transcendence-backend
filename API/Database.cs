namespace KeepGrouped.API;

using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;
using KeepGrouped.API.Events;
using KeepGrouped.API.Storage;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Event>().HasMany(e => e.Users).WithMany(e => e.Events).UsingEntity<Registration>();
        builder.Entity<Event>().HasOne(e => e.Organizer);
        builder.Entity<File>().HasOne(f => f.Creator);
        builder.Entity<EventRole>().HasAlternateKey(er => er.Name);

        builder.Entity<Event>().Navigation(e => e.Organizer).AutoInclude();
        builder.Entity<Event>().Navigation(e => e.Registrations).AutoInclude();
        builder.Entity<Event>().Navigation(e => e.EventRoles).AutoInclude();
        builder.Entity<Registration>().Navigation(e => e.User).AutoInclude();
        builder.Entity<File>().Navigation(f => f.Creator).AutoInclude();
    }
    public DbSet<User> Users {get; set;}
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRole> EventRoles { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

}

