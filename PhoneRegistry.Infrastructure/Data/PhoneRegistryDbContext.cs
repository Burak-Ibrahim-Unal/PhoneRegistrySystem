using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Domain.Entities;

namespace PhoneRegistry.Infrastructure.Data;

public class PhoneRegistryDbContext : DbContext
{
    public PhoneRegistryDbContext(DbContextOptions<PhoneRegistryDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<ContactInfo> ContactInfos { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<LocationStatistic> LocationStatistics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Person Configuration
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Company).HasMaxLength(100);
            entity.HasMany(e => e.ContactInfos).WithOne(e => e.Person).HasForeignKey(e => e.PersonId);
        });

        // ContactInfo Configuration
        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired();
        });

        // Report Configuration
        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired();
            entity.HasMany(e => e.LocationStatistics).WithOne(e => e.Report).HasForeignKey(e => e.ReportId);
        });

        // LocationStatistic Configuration
        modelBuilder.Entity<LocationStatistic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
        });

        // Global query filters for soft delete
        modelBuilder.Entity<Person>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<ContactInfo>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Report>().HasQueryFilter(r => !r.IsDeleted);
    }
}
