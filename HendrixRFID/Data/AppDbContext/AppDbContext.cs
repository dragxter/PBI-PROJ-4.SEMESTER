using Microsoft.EntityFrameworkCore;
using HendrixRFID.Models;

namespace HendrixRFID.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pig> Pigs => Set<Pig>();
    public DbSet<Lamp> Lamps => Set<Lamp>();
    public DbSet<RawScan> RawScans => Set<RawScan>();
}