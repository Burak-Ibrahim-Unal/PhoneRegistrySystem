using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Domain.ValueObjects;
using PhoneRegistry.Infrastructure.Data;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Models;
using System.Text.Json;

namespace PhoneRegistry.WorkerService.Services;

public class ContactEventsConsumer : IMessageConsumer<IntegrationEventEnvelope>
{
    private readonly ReportDbContext _reportDb;
    private readonly ILogger<ContactEventsConsumer> _logger;

    public ContactEventsConsumer(ReportDbContext reportDb, ILogger<ContactEventsConsumer> logger)
    {
        _reportDb = reportDb;
        _logger = logger;
    }

    public async Task ConsumeAsync(IntegrationEventEnvelope message, CancellationToken cancellationToken = default)
    {
        try
        {
            switch (message.EventType)
            {
                case "ContactInfoUpserted":
                    await HandleContactInfoUpserted(message.Payload, cancellationToken);
                    break;
                case "ContactInfoDeleted":
                    await HandleContactInfoDeleted(message.Payload, cancellationToken);
                    break;
                case "PersonUpserted":
                    // Person bazlı bir özet gerekirse burada ele alınır
                    break;
                default:
                    _logger.LogWarning("Unknown event type: {EventType}", message.EventType);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error consuming contact event: {EventType}", message.EventType);
            throw;
        }
    }

    private async Task HandleContactInfoUpserted(JsonElement payload, CancellationToken ct)
    {
        var personId = payload.GetProperty("PersonId").GetGuid();
        var type = (ContactType)payload.GetProperty("Type").GetInt32();
        if (type != ContactType.Location) return;

        var cityName = payload.TryGetProperty("CityName", out var cityEl) && cityEl.ValueKind != JsonValueKind.Null
            ? cityEl.GetString()
            : payload.GetProperty("Content").GetString();
        if (string.IsNullOrWhiteSpace(cityName)) return;

        // Basit strateji: ilgili şehir için LocationStatistic'i güncelle
        var stat = await _reportDb.LocationStatistics.FirstOrDefaultAsync(s => s.Location == cityName, ct);
        if (stat == null)
        {
            stat = new LocationStatistic(cityName!, 1, 0);
            await _reportDb.LocationStatistics.AddAsync(stat, ct);
        }
        else
        {
            stat = new LocationStatistic(stat.Location, stat.PersonCount + 1, stat.PhoneNumberCount);
            _reportDb.Entry(stat).State = EntityState.Added; // basit insert; gelişmiş stratejide update edilir
        }

        await _reportDb.SaveChangesAsync(ct);
        _logger.LogInformation("Upserted LocationStatistic for city {CityName}", cityName);
    }

    private async Task HandleContactInfoDeleted(JsonElement payload, CancellationToken ct)
    {
        var type = ContactType.Location; // sadece lokasyonları etkili sayalım
        var personId = payload.GetProperty("PersonId").GetGuid();
        var contactInfoId = payload.GetProperty("ContactInfoId").GetGuid();
        // Basitleştirilmiş: cityName yoksa no-op
        // Gerçek senaryoda projection store’da kişi-şehir mapping’i tutulur ve ona göre decrement yapılır
        _logger.LogInformation("ContactInfoDeleted received for {PersonId} - projection decrement requires mapping store (TODO)", personId);
        await Task.CompletedTask;
    }
}


