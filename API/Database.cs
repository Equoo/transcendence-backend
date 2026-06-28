namespace KeepGrouped.API;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KeepGrouped.API.Users;
using KeepGrouped.API.Events;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Event>().HasMany(e => e.Users).WithMany(e => e.Events).UsingEntity<Registration>();
        builder.Entity<Event>().HasOne(e => e.Organizer);

    }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRole> EventRoles { get; set; }
}