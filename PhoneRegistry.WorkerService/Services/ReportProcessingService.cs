using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Models;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.WorkerService.Services;

public class ReportProcessingService : IMessageConsumer<ReportRequestMessage>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PhoneRegistryDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ReportProcessingService> _logger;

    public ReportProcessingService(
        IUnitOfWork unitOfWork,
        PhoneRegistryDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<ReportProcessingService> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

        // Lokasyon istatistiklerini hesapla (Contact API üzerinden)
        var locationStatistics = await CalculateLocationStatisticsViaHttpAsync(cancellationToken);

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

    private async Task<List<LocationStatistic>> CalculateLocationStatisticsViaHttpAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating location statistics via Contact API...");

        var baseUrl = _configuration["ContactApi:BaseUrl"] ?? "http://localhost:5000";
        var client = _httpClientFactory.CreateClient("contact-api");
        client.BaseAddress = new Uri(baseUrl);

        var allPersons = new List<PersonDto>();
        int skip = 0;
        const int take = 200;
        while (true)
        {
            var page = await client.GetFromJsonAsync<List<PersonDto>>($"/api/persons?skip={skip}&take={take}", cancellationToken);
            if (page == null || page.Count == 0) break;
            allPersons.AddRange(page);
            skip += take;
            if (page.Count < take) break;
        }

        _logger.LogInformation("Fetched {Count} persons from Contact API", allPersons.Count);

        var locationContacts = allPersons
            .SelectMany(p => p.contactInfos
                .Where(ci => ci.type == (int)ContactType.Location && !ci.isDeleted)
                .Select(ci => new {
                    PersonId = p.id,
                    Location = !string.IsNullOrWhiteSpace(ci.cityName) ? ci.cityName : ci.content
                }))
            .ToList();

        var locationGroups = locationContacts
            .GroupBy(x => x.Location, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var statistics = new List<LocationStatistic>();
        foreach (var group in locationGroups)
        {
            var location = group.Key;
            var personIdsInLocation = group.Select(x => x.PersonId).Distinct().ToList();

            var personCount = personIdsInLocation.Count;
            var phoneNumberCount = allPersons
                .Where(p => personIdsInLocation.Contains(p.id))
                .SelectMany(p => p.contactInfos)
                .Count(ci => ci.type == (int)ContactType.PhoneNumber && !ci.isDeleted);

            statistics.Add(new LocationStatistic(location, personCount, phoneNumberCount));
        }

        _logger.LogInformation("Calculated statistics for {LocationCount} locations", statistics.Count);
        return statistics;
    }

    private record PersonDto(Guid id, string firstName, string lastName, string? company, List<ContactInfoDto> contactInfos);
    private record ContactInfoDto(Guid id, int type, string content, bool isDeleted, string? cityName);
}
