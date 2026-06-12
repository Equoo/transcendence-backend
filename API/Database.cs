namespace KeepGrouped.API;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KeepGrouped.API.Users;
using KeepGrouped.API.Events;

class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Event>().HasMany(e => e.Users).WithMany(e => e.Events).UsingEntity<Registration>();

    }
    public DbSet<Event> Evenements { get; set; }
}