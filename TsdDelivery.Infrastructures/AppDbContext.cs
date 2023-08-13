using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TsdDelivery.Domain.Entities;

namespace TsdDelivery.Infrastructures;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<User> Users { get; set; }

}
