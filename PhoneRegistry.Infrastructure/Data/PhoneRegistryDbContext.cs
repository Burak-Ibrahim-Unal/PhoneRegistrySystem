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
    public DbSet<City> Cities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Person Configuration
        modelBuilder.Entity<Person>(entity =>
        {
            entity.ToTable("Persons", schema: "contact");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Company).HasMaxLength(100);
            entity.HasMany(e => e.ContactInfos).WithOne(e => e.Person).HasForeignKey(e => e.PersonId);
        });

        // ContactInfo Configuration
        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.ToTable("ContactInfos", schema: "contact");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Content).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Type).IsRequired();
            // City ilişkisi (opsiyonel)
            entity.HasOne(e => e.City)
                .WithMany(c => c.ContactInfos)
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Report Configuration
        modelBuilder.Entity<Report>(entity =>
        {
            entity.ToTable("Reports", schema: "report");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Status).IsRequired();
            entity.HasMany(e => e.LocationStatistics).WithOne(e => e.Report).HasForeignKey(e => e.ReportId);
        });

        // LocationStatistic Configuration
        modelBuilder.Entity<LocationStatistic>(entity =>
        {
            entity.ToTable("LocationStatistics", schema: "report");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
        });

        // City Configuration
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("Cities", schema: "contact");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
        });

        // Global query filters for soft delete - geçici olarak kaldırıldı
        // Navigation property sorunları nedeniyle manuel filtering yapacağız
        // modelBuilder.Entity<Person>().HasQueryFilter(p => !p.IsDeleted);
        // modelBuilder.Entity<ContactInfo>().HasQueryFilter(c => !c.IsDeleted);
        // modelBuilder.Entity<Report>().HasQueryFilter(r => !r.IsDeleted);
    }
}
