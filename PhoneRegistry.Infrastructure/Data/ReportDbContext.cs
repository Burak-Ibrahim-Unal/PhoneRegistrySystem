using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Infrastructure.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) { }

    public DbSet<Report> Reports { get; set; }
    public DbSet<LocationStatistic> LocationStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("report");

        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).IsRequired();
            entity.HasMany(e => e.LocationStatistics).WithOne(e => e.Report).HasForeignKey(e => e.ReportId);
        });

        modelBuilder.Entity<LocationStatistic>(entity =>
        {
            entity.ToTable("LocationStatistics");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
        });
    }
}


