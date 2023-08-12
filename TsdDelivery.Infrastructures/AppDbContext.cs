using Microsoft.EntityFrameworkCore;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<User> Users { get; set; }


}
