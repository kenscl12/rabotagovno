using Microsoft.EntityFrameworkCore;
using RabotaGovnoClone.Models;

namespace RabotaGovnoClone.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Comment> Comments => Set<Comment>();
}