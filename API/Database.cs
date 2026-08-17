namespace KeepGrouped.API;

using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
<<<<<<< HEAD
using KeepGrouped.API.Password;
using KeepGrouped.API.Storage;
using KeepGrouped.API.Users;
using KeepGrouped.API.Users.Invitation;
using Microsoft.EntityFrameworkCore;

public class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : DbContext(options)
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
		builder.Entity<StorageFile>().HasOne(f => f.Creator);
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

			// Speeds up: WHERE ChannelId = X ORDER BY SentAt DESC
			entity
				.HasIndex(m => new { m.ChannelId, m.SentAt })
				.HasDatabaseName("IX_Messages_ChannelId_SentAt");

			entity.Property(m => m.Content).HasMaxLength(4096);
		});
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<EventRole> EventRoles { get; set; }
	public DbSet<StorageFile> Files { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<Invitation> Invitations { get; set; }
	public DbSet<Channel> Channels { get; set; }
	public DbSet<Message> Messages { get; set; }
}

public static class DbBuilder
{
	public static void BuildDb(this WebApplicationBuilder builder)
	{
		builder.Services.AddDbContext<KeepGroupedDb>(options =>
			options
				.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
				.UseSeeding(
					(db, _) =>
					{
						if (db.Set<User>().FirstOrDefault(u => u.UserName == "asventi") != null)
						{
							return;
						}
						User user = new("asventi");

						user.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(
							user,
							"1234"
						);
						db.Set<User>().Add(user);
						db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
						db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
						db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
						db.Set<EventRole>().Add(new EventRole() { Name = EventRole.Implicit });
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
						ev.EventRoles.Add(
							db.Set<EventRole>().First(er => er.Name == EventRole.Implicit)
						);
						db.Set<Event>().Add(ev);

						db.SaveChanges();
					}
				)
		);
	}
}
=======
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

            // Speeds up: WHERE ChannelId = X ORDER BY SentAt DESC
            entity
                .HasIndex(m => new { m.ChannelId, m.SentAt })
                .HasDatabaseName("IX_Messages_ChannelId_SentAt");

            entity.Property(m => m.Content).HasMaxLength(4096);
        });

        builder.Entity<Channel>(entity =>
        {
            entity.Property(c => c.Id).HasMaxLength(36);

            entity.HasIndex(c => c.Name);
        });
    }

    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<EventRole> EventRoles { get; set; } = null!;
    public DbSet<Channel> Channels { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
}
>>>>>>> f0607d4f1b8779fb0340fd949682b9a1f313e36c
