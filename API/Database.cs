namespace KeepGrouped.API;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using KeepGrouped.API.Users;

class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : IdentityDbContext<ApplicationUser>(options)
{
}