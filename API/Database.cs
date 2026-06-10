namespace KeepGrouped.API;

using KeepGrouped.API.Users;
using Microsoft.EntityFrameworkCore;

class KeepGroupedDb(DbContextOptions<KeepGroupedDb> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
}