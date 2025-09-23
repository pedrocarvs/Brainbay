using Microsoft.EntityFrameworkCore;
using Brainbay.Core.Models;

namespace Brainbay.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Character> Characters { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}