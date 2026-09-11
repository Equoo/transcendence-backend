namespace KeepGrouped.API;

using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Users;
using KeepGrouped.API.Events;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Password;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users.Roles;
using KeepGrouped.API.Users.Invitation;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : DbContext(options)
{

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Event>().HasMany(e => e.Users).WithMany(e => e.Events).UsingEntity<Registration>();
        builder.Entity<Event>().HasOne(e => e.Organizer);
        builder.Entity<EventRole>().HasAlternateKey(er => er.Name);

        builder.Entity<StorageFile>().HasOne(f => f.Creator);
        builder.Entity<User>().HasOne(u => u.Avatar).WithOne().HasForeignKey<User>("AvatarKey");

    }
    public DbSet<User> Users { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventRole> EventRoles { get; set; }
    public DbSet<StorageFile> Files { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<Role> Roles { get; set; }
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
            User user = new() { UserName = "asventi", Role = new("Lautre", 1) };

            user.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user, "1234");
            db.Set<User>().Add(user);

            db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
            db.Set<EventRole>().Add(new EventRole() { Name = EventRole.Implicit });

            db.SaveChanges();

            User user5 = new() { UserName = "a", Role = new("Admin", 1) };
            User user1 = new() { UserName = "devan", Role = new("Modo", 1) };
            User user2 = new() { UserName = "pierre", Role = new("Helper", 1) };
            User user3 = new() { UserName = "tom", Role = new("Gold", 1) };
            User user4 = new() { UserName = "david", Role = new("Member", 1) };

            user5.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user5, "a");
            user1.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user1, "a");
            user2.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user2, "a");
            user3.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user3, "a");
            user4.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user4, "a");

            db.Set<User>().Add(user5);
            db.Set<User>().Add(user1);
            db.Set<User>().Add(user2);
            db.Set<User>().Add(user3);
            db.Set<User>().Add(user4);

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
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == EventRole.Implicit));
            db.Set<Event>().Add(ev);

            db.SaveChanges();
        }));
    }

}

