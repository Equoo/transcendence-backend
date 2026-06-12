using KeepGrouped.API.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KeepGrouped.API.Users;

class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
    }
}

[EntityTypeConfiguration(typeof(ApplicationUserConfiguration))]
public class ApplicationUser : IdentityUser
{
    public ApplicationUser() : base() { }
    public ApplicationUser(string username) : base(username) { }

    public ICollection<Event> Events { get; } = [];
    public ICollection<Registration> Registrations { get; } = [];
}