namespace KeepGrouped.API;

using KeepGrouped.API.Chat;
using KeepGrouped.API.Events;
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

		builder.Entity<ChannelAck>().HasKey(a => new { a.UserId, a.ChannelId });

		builder.Entity<ChannelAck>().HasOne(m => m.Channel)
			.WithMany()
			.HasForeignKey(m => m.ChannelId)
			.OnDelete(DeleteBehavior.Cascade);
	}

	public DbSet<User> Users { get; set; }
	public DbSet<Event> Events { get; set; }
	public DbSet<EventRole> EventRoles { get; set; }
	public DbSet<StorageFile> Files { get; set; }
	public DbSet<RefreshToken> RefreshTokens { get; set; }
	public DbSet<Invitation> Invitations { get; set; }
	public DbSet<Channel> Channels { get; set; }
	public DbSet<ChannelAck> ChannelAcks { get; set; }
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

						User user2 = new("equo");
						user2.PasswordHash = new KeepGroupedPasswordHasher().HashPassword(
							user2,
							"feur"
						);
						db.Set<User>().Add(user2);

						db.Set<EventRole>().Add(new EventRole() { Name = "DPS" });
						db.Set<EventRole>().Add(new EventRole() { Name = "Heal" });
						db.Set<EventRole>().Add(new EventRole() { Name = "Tank" });
						db.Set<EventRole>().Add(new EventRole() { Name = EventRole.Implicit });
						db.SaveChanges();

						db.SaveChanges();
					}
				)
		);
	}
}
