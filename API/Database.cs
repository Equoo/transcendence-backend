namespace KeepGrouped.API;

using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
using KeepGrouped.API.Password;
using KeepGrouped.API.Roles;
using KeepGrouped.API.Users.Invitation;
using KeepGrouped.API.Users.Auth;
using Microsoft.EntityFrameworkCore;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Event>(entity =>
        {

            entity.HasMany(e => e.Users)
                .WithMany(e => e.Events)
                .UsingEntity<Registration>();

            entity.HasOne(e => e.Organizer);

            entity.HasOne(e => e.Channel)
                .WithOne(c => c.Event)
                .HasForeignKey<Event>(e => e.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<StorageFile>().HasOne(f => f.Creator);
        builder.Entity<User>(entity =>
        {
            entity.HasOne(u => u.Avatar).WithOne().HasForeignKey<User>("AvatarKey");
            entity.Navigation(u => u.Avatar).AutoInclude();
            entity.Navigation(u => u.Role).AutoInclude();
        });
        builder.Entity<EventRole>().HasAlternateKey(er => er.Name);

        builder.Entity<Message>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);

            entity
                .HasOne(m => m.Channel)
                .WithMany()
                .HasForeignKey(m => m.ChannelId)
                .OnDelete(DeleteBehavior.Cascade);

            entity
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity
                .HasOne(m => m.MessageRef)
                .WithMany()
                .HasForeignKey(m => m.MessageRefId)
                .OnDelete(DeleteBehavior.SetNull);

            // Speeds up: WHERE ChannelId = X ORDER BY SentAt DESC
            entity
                .HasIndex(m => new { m.ChannelId, m.SentAt })
                .HasDatabaseName("IX_Messages_ChannelId_SentAt");

            entity.Property(m => m.Content).HasMaxLength(8192);
        });

        builder.Entity<Channel>(entity =>
        {
            entity
                .HasOne<ChannelCategory>()
                .WithMany()
                .HasForeignKey(c => c.Category)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<ChannelCategory>().HasAlternateKey(c => c.Name);

        builder.Entity<ChannelAck>().HasKey(a => new { a.UserId, a.ChannelId });

        builder.Entity<ChannelAck>().HasOne(m => m.Channel)
            .WithMany()
            .HasForeignKey(m => m.ChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventRole> EventRoles { get; set; } = null!;
    public DbSet<StorageFile> Files { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Invitation> Invitations { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<ChannelCategory> ChannelCategories { get; set; } = null!;
    public DbSet<ChannelAck> ChannelAcks { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
}

public static class DbBuilder
{
    static public void BuildDb(this WebApplicationBuilder builder)
    {
        var authOptions = builder.Configuration
            .GetSection(AuthenticationOptions.SectionName)
            .Get<AuthenticationOptions>()!;
        builder.Services.AddDbContext<KeepGroupedDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSeeding((db, _) =>
        {
            if (db.Set<User>().FirstOrDefault(u => u.UserName == authOptions.DefaultAdminLogin) != null)
            {
                return;
            }
            User user = new() { UserName = authOptions.DefaultAdminLogin, Role = new("SuperAdmin", int.MaxValue) };

            user.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(user, authOptions.DefaultAdminPwd);
            db.Set<User>().Add(user);
            db.Set<EventRole>().Add(new EventRole() { Name = EventRole.Implicit });
            db.Set<Role>().Add(new Role("Member"));
            db.SaveChanges();

            if (!builder.Environment.IsDevelopment())
            {
                return;
            }

            db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
            db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
            User user5 = new() { UserName = "a", Role = new("Admin", int.MaxValue) };
            User user1 = new() { UserName = "devan", Role = new("Modo", 1) };
            User user2 = new() { UserName = "pierre", Role = new("Helper", 1) };
            User user3 = new() { UserName = "tom", Role = new("Gold", int.MaxValue) };
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
            };
            var evChan = new Channel("default-event", "", ev.Id);
            ev.Channel = evChan;
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "DPS"));
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == "Heal"));
            ev.EventRoles.Add(db.Set<EventRole>().First(er => er.Name == EventRole.Implicit));
            db.Set<Event>().Add(ev);

            db.SaveChanges();
        }));
    }

}
