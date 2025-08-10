using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;

namespace PhoneRegistry.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedCitiesAsync(PhoneRegistryDbContext context, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        var defaultCities = new[] { "Ankara", "İstanbul", "İzmir" };

        var existing = await context.Cities
            .Select(c => c.Name)
            .ToListAsync(cancellationToken);

        var toAdd = defaultCities
            .Where(name => !existing.Contains(name))
            .Select(name => new City(name))
            .ToList();

        if (toAdd.Count == 0)
        {
            logger?.LogInformation("City seed skipped. All default cities already exist.");
            return;
        }

        await context.Cities.AddRangeAsync(toAdd, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        logger?.LogInformation("Seeded default cities: {Cities}", string.Join(", ", toAdd.Select(c => c.Name)));
    }

    public static async Task SeedDemoDataAsync(PhoneRegistryDbContext context, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        // Eğer zaten kişi varsa, demo seed atla
        var hasPersons = await context.Persons.AnyAsync(cancellationToken);
        if (hasPersons)
        {
            logger?.LogInformation("Demo data seed skipped. Persons already exist.");
            return;
        }

        // Şehirleri al
        var cities = await context.Cities.ToListAsync(cancellationToken);
        if (cities.Count == 0)
        {
            await SeedCitiesAsync(context, logger, cancellationToken);
            cities = await context.Cities.ToListAsync(cancellationToken);
        }

        City cityAnkara = cities.First(c => c.Name.Equals("Ankara", StringComparison.OrdinalIgnoreCase));
        City cityIstanbul = cities.First(c => c.Name.Equals("İstanbul", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("Istanbul", StringComparison.OrdinalIgnoreCase));
        City cityIzmir = cities.First(c => c.Name.Equals("İzmir", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("Izmir", StringComparison.OrdinalIgnoreCase));

        // Kişiler
        var persons = new List<Person>
        {
            new Person("Ali", "Yılmaz", "ABC AŞ"),
            new Person("Ayşe", "Demir", "XYZ Ltd"),
            new Person("Mehmet", "Kaya", "KLM AŞ"),
            new Person("Zeynep", "Çelik", "ACME"),
            new Person("Ahmet", "Arslan", "Contoso"),
            new Person("Elif", "Koç", "Fabrikam"),
            new Person("Can", "Şahin", "Northwind"),
            new Person("Deniz", "Güneş", "Tailspin"),
            new Person("Burak", "Ünal", "AdventureWorks"),
            new Person("Selin", "Ak", "Wingtip"),
        };

        await context.Persons.AddRangeAsync(persons, cancellationToken);
        await context.SaveChangesAsync(cancellationToken); // Id'ler üretildi

        // İletişim bilgileri (telefon, email, şehir)
        var contactInfos = new List<ContactInfo>();
        string[] phones = { "+905301112233", "+905061234567", "+905071112233", "+905531234567", "+905421112233", "+905351234567" };
        string[] emails = { "ali@example.com", "ayse@example.com", "mehmet@example.com", "zeynep@example.com", "ahmet@example.com", "elif@example.com" };

        int i = 0;
        foreach (var p in persons)
        {
            // Telefon
            contactInfos.Add(new ContactInfo(p.Id, ContactType.PhoneNumber, phones[i % phones.Length]));
            // Email
            contactInfos.Add(new ContactInfo(p.Id, ContactType.EmailAddress, emails[i % emails.Length]));
            // Şehir (lokasyon)
            var city = (i % 3) switch
            {
                0 => cityAnkara,
                1 => cityIstanbul,
                _ => cityIzmir
            };
            contactInfos.Add(new ContactInfo(p.Id, ContactType.Location, city.Name, city.Id));
            i++;
        }

        await context.ContactInfos.AddRangeAsync(contactInfos, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Raporlar: 1 Completed (istatistiklerle), 1 Preparing, 1 Failed
        // Completed rapor ve istatistikleri hesapla
        var reportCompleted = new Report();

        var personWithContacts = await context.Persons
            .Include(p => p.ContactInfos)
            .ThenInclude(ci => ci.City)
            .ToListAsync(cancellationToken);

        var locationContacts = personWithContacts
            .SelectMany(p => p.ContactInfos
                .Where(ci => ci.Type == ContactType.Location && !ci.IsDeleted)
                .Select(ci => new {
                    Person = p,
                    Location = ci.City != null && !string.IsNullOrWhiteSpace(ci.City.Name)
                        ? ci.City.Name
                        : ci.Content
                }))
            .ToList();

        var locationGroups = locationContacts
            .GroupBy(x => x.Location, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var statistics = new List<LocationStatistic>();
        foreach (var group in locationGroups)
        {
            var location = group.Key;
            var personsInLocation = group.Select(x => x.Person).Distinct().ToList();
            var personCount = personsInLocation.Count;
            var phoneNumberCount = personsInLocation
                .SelectMany(p => p.ContactInfos)
                .Where(ci => ci.Type == ContactType.PhoneNumber && !ci.IsDeleted)
                .Count();

            statistics.Add(new LocationStatistic(location, personCount, phoneNumberCount));
        }

        // Önce raporu kaydet ki Report.Id üretilebilsin
        await context.Reports.AddAsync(reportCompleted, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Ardından istatistikleri ilişkilendir
        reportCompleted.CompleteReport(statistics);
        await context.LocationStatistics.AddRangeAsync(statistics, cancellationToken);

        // Preparing
        var reportPreparing = new Report();
        await context.Reports.AddAsync(reportPreparing, cancellationToken);

        // Failed
        var reportFailed = new Report();
        reportFailed.FailReport("Seed failure example");
        await context.Reports.AddAsync(reportFailed, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        logger?.LogInformation("Demo data seeded: {PersonCount} persons, {ContactCount} contacts, 3 reports (1 completed)", persons.Count, contactInfos.Count);
    }

    public static async Task SeedFixedReportAsync(PhoneRegistryDbContext context, ILogger? logger = null, CancellationToken cancellationToken = default)
    {
        var fixedId = Guid.Parse("0656072a-6b3f-4073-a26d-5b49f6f1e690");
        var exists = await context.Reports.AnyAsync(r => r.Id == fixedId, cancellationToken);
        if (exists)
        {
            logger?.LogInformation("Fixed report already exists: {ReportId}", fixedId);
            return;
        }

        var sql = @"INSERT INTO public.""Reports""
        (""Id"", ""RequestedAt"", ""Status"", ""CompletedAt"", ""ErrorMessage"", ""CreatedAt"", ""UpdatedAt"", ""IsDeleted"")
        VALUES('0656072a-6b3f-4073-a26d-5b49f6f1e690'::uuid, TIMESTAMP '2025-08-11 01:12:37.247', 2, TIMESTAMP '2025-08-11 01:12:37.582', NULL, TIMESTAMP '2025-08-11 01:12:37.247', TIMESTAMP '2025-08-11 01:12:37.582', FALSE);";

        await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        logger?.LogInformation("Seeded fixed report: {ReportId}", fixedId);
    }
}


