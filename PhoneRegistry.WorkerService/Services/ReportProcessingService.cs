using Microsoft.Extensions.Logging;
using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Infrastructure.Messaging.Interfaces;

namespace PhoneRegistry.WorkerService.Services;

public class ReportProcessingService : IMessageConsumer<ReportRequestMessage>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportProcessingService> _logger;

    public ReportProcessingService(IUnitOfWork unitOfWork, ILogger<ReportProcessingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ConsumeAsync(ReportRequestMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing report request for Report ID: {ReportId}", message.ReportId);

        try
        {
            // Report'u getir
            var report = await _unitOfWork.Reports.GetByIdAsync(message.ReportId, cancellationToken);
            if (report == null)
            {
                _logger.LogError("Report not found: {ReportId}", message.ReportId);
                return;
            }

            // Lokasyon istatistiklerini hesapla
            var locationStatistics = await CalculateLocationStatisticsAsync(cancellationToken);

            // Report'u tamamla
            report.CompleteReport(locationStatistics);

            // Veritabanını güncelle
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Report {ReportId} completed successfully with {LocationCount} locations", 
                message.ReportId, locationStatistics.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process report {ReportId}", message.ReportId);

            try
            {
                // Report'u fail olarak işaretle
                var report = await _unitOfWork.Reports.GetByIdAsync(message.ReportId, cancellationToken);
                if (report != null)
                {
                    report.FailReport(ex.Message);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception failEx)
            {
                _logger.LogError(failEx, "Failed to mark report {ReportId} as failed", message.ReportId);
            }
        }
    }

    private async Task<List<LocationStatistic>> CalculateLocationStatisticsAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Calculating location statistics...");

        // Tüm kişileri contact info'larıyla birlikte getir
        var persons = await _unitOfWork.Persons.GetAllWithContactInfosAsync();

        // Lokasyon bilgilerini grupla
        var locationGroups = persons
            .SelectMany(p => p.ContactInfos
                .Where(ci => ci.Type == ContactType.Location && !ci.IsDeleted)
                .Select(ci => new { Person = p, Location = ci.Content }))
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

            statistics.Add(new LocationStatistic(Guid.Empty, location, personCount, phoneNumberCount));

            _logger.LogDebug("Location: {Location}, Persons: {PersonCount}, Phone Numbers: {PhoneCount}",
                location, personCount, phoneNumberCount);
        }

        _logger.LogInformation("Calculated statistics for {LocationCount} locations", statistics.Count);
        return statistics;
    }
}
