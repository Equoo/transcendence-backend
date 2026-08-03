namespace KeepGrouped.API;

using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;
using KeepGrouped.API.Events;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Password;

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
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRole> EventRoles { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
}

static public class DbBuilder
{
    static public void BuildDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSeeding((db, _) =>
        {
            if (db.Set<User>().FirstOrDefault(u => u.UserName == "asventi") != null)
            {
                return;
            }
            User user = new("asventi");

            user.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user, "1234");
            db.Set<User>().Add(user);
            db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Any" });
            db.SaveChanges();

            var ev = new Event()
            {
                Name = "Default Event",
                Date = DateTime.UtcNow.AddMinutes(30),
                Location = "Default Location",
                Size = 10,
                Organizer = user,
                // EventRoles = [.. db.Set<EventRole>()]
            };
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "DPS"));
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "Heal"));
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "Any"));
            db.Set<Event>().Add(ev);

            db.SaveChanges();
        }));
    }
}

