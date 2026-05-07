namespace HendrixRFID.Data;
using HendrixRFID.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Stable> Stables => Set<Stable>();
    public DbSet<Pig> Pigs => Set<Pig>();
    public DbSet<Lamp> Lamps => Set<Lamp>();
    public DbSet<PigLocation> PigLocations => Set<PigLocation>();
    public DbSet<PigHistory> PigHistories => Set<PigHistory>();
    public DbSet<RawScan> RawScans => Set<RawScan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Pig => Stable (BelongsTo)
        modelBuilder.Entity<Pig>()
            .HasOne(p => p.Stable)
            .WithMany()
            .HasForeignKey(p => p.BelongsTo);

        // Lamp => Stable (PlacedIn)
        modelBuilder.Entity<Lamp>()
            .HasOne(l => l.Stable)
            .WithMany()
            .HasForeignKey(l => l.PlacedIn);

        // PigLocation => Pig (1-til-1)
        modelBuilder.Entity<PigLocation>()
            .HasOne(pl => pl.Pig)
            .WithOne()
            .HasForeignKey<PigLocation>(pl => pl.PigId);

        // PigLocation => Lamp
        modelBuilder.Entity<PigLocation>()
            .HasOne(pl => pl.Lamp)
            .WithMany()
            .HasForeignKey(pl => pl.CurrentLampId);

        // PigHistory => Pig
        modelBuilder.Entity<PigHistory>()
            .HasOne(ph => ph.Pig)
            .WithMany()
            .HasForeignKey(ph => ph.PigId);

        // PigHistory => Lamp 
        modelBuilder.Entity<PigHistory>()
            .HasOne(ph => ph.Lamp)
            .WithMany()
            .HasForeignKey(ph => ph.LampId);

        // RawScan => Pig
        modelBuilder.Entity<RawScan>()
            .HasOne(r => r.Pig)
            .WithMany()
            .HasForeignKey(r => r.PigId);

        // RawScan => Lamp
        modelBuilder.Entity<RawScan>()
            .HasOne(r => r.Lamp)
            .WithMany()
            .HasForeignKey(r => r.LampId);
    }
}