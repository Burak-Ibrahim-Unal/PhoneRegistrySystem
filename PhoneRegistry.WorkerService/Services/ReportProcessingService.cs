using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Infrastructure.Messaging.Interfaces;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.WorkerService.Services;

public class ReportProcessingService : IMessageConsumer<ReportRequestMessage>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PhoneRegistryDbContext _context;
    private readonly ILogger<ReportProcessingService> _logger;

    public ReportProcessingService(
        IUnitOfWork unitOfWork, 
        PhoneRegistryDbContext context, 
        ILogger<ReportProcessingService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ConsumeAsync(ReportRequestMessage message, CancellationToken cancellationToken = default)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logger.LogInformation("Processing report request for Report ID: {ReportId}", message.ReportId);

        try
        {
            await ProcessReportAsync(message.ReportId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process report {ReportId}", message.ReportId);
            await HandleProcessingErrorAsync(message.ReportId, ex, cancellationToken);
        }
    }

    private async Task ProcessReportAsync(Guid reportId, CancellationToken cancellationToken)
    {
        // Her bir mesaj işlenirken önce takip edilen state'i temizle (stale tracking önlenir)
        _context.ChangeTracker.Clear();
        
        // Report'u getir
        var report = await _unitOfWork.Reports.GetByIdAsync(reportId, cancellationToken);
        if (report == null)
        {
            _logger.LogError("Report not found: {ReportId}", reportId);
            return;
        }

        // Lokasyon istatistiklerini hesapla
        var locationStatistics = await CalculateLocationStatisticsAsync(cancellationToken);

        // Domain yöntemi ile raporu tamamla (durum geçişi + ilişkiler tek yerden yönetilir)
        report.CompleteReport(locationStatistics);

        // Yeni oluşturulan LocationStatistic nesnelerini açıkça Added olarak işaretle
        _context.LocationStatistics.AddRange(locationStatistics);
        
        // Explicit: EF durumunu Added olarak zorla (UPDATE üretilmesini engeller)
        foreach (var stat in locationStatistics)
        {
            _context.Entry(stat).State = EntityState.Added;
        }

        // Veritabanını güncelle
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Report {ReportId} completed successfully with {LocationCount} locations", 
            reportId, locationStatistics.Count);
    }

    private async Task HandleProcessingErrorAsync(Guid reportId, Exception ex, CancellationToken cancellationToken)
    {
        try
        {
            // Stale tracking'i temizle ve raporu tekrar yükleyip sadece hala Preparing ise fail'e çek
            _context.ChangeTracker.Clear();
            var report = await _unitOfWork.Reports.GetByIdAsync(reportId, cancellationToken);
            if (report != null && report.Status == ReportStatus.Preparing)
            {
                report.FailReport(ex.Message);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            else if (report != null)
            {
                _logger.LogWarning("Report {ReportId} is in status {Status} after failure; skipping fail transition.", 
                    reportId, report.Status);
            }
        }
        catch (Exception failEx)
        {
            _logger.LogError(failEx, "Failed to mark report {ReportId} as failed", reportId);
        }
    }

    private async Task<List<LocationStatistic>> CalculateLocationStatisticsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating location statistics...");

        // Tüm kişileri contact info'larıyla birlikte getir
        var persons = await _unitOfWork.Persons.GetAllWithContactInfosAsync(cancellationToken);
        
        _logger.LogInformation("Found {PersonCount} persons in database", persons.Count());

        // Lokasyon bilgilerini grupla
        var locationContacts = persons
            .SelectMany(p => p.ContactInfos
                .Where(ci => ci.Type == ContactType.Location && !ci.IsDeleted)
                .Select(ci => new { 
                    Person = p, 
                    Location = ci.City != null && !string.IsNullOrWhiteSpace(ci.City.Name)
                        ? ci.City.Name
                        : ci.Content
                }))
            .ToList();
            
        _logger.LogInformation("Found {LocationContactCount} location contacts", locationContacts.Count);
        
        var locationGroups = locationContacts
            .GroupBy(x => x.Location, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var statistics = new List<LocationStatistic>();

        foreach (var locationGroup in locationGroups)
        {
            var location = locationGroup.Key;
            var personsInLocation = locationGroup.Select(x => x.Person).Distinct().ToList();
            
            // O lokasyondaki kişi sayısı
            var personCount = personsInLocation.Count;
            
            // O lokasyondaki telefon numarası sayısı
            var phoneNumberCount = personsInLocation
                .SelectMany(p => p.ContactInfos)
                .Where(ci => ci.Type == ContactType.PhoneNumber && !ci.IsDeleted)
                .Count();

            var stat = new LocationStatistic(location, personCount, phoneNumberCount);
            statistics.Add(stat);

            _logger.LogDebug("Location: {Location}, Persons: {PersonCount}, Phone Numbers: {PhoneCount}",
                location, personCount, phoneNumberCount);
        }

        _logger.LogInformation("Calculated statistics for {LocationCount} locations", statistics.Count);
        return statistics;
    }
}
