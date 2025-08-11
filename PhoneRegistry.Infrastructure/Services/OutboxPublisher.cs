using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Domain.Entities;
using PhoneRegistry.Messaging.Services;
using System.Text.Json;
using PhoneRegistry.Infrastructure.Repositories;
using PhoneRegistry.Messaging.Interfaces;

namespace PhoneRegistry.Infrastructure.Services;

public class OutboxPublisher : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<OutboxPublisher> _logger;

    public OutboxPublisher(IServiceProvider services, ILogger<OutboxPublisher> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<OutboxRepository>();
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                var pending = await repo.GetPendingBatchAsync(100, stoppingToken);
                foreach (var msg in pending)
                {
                    try
                    {
                        msg.MarkProcessing();
                        var queue = ResolveQueueName(msg.EventType);
                        await publisher.PublishAsync(JsonSerializer.Deserialize<object>(msg.Payload)!, queue, stoppingToken);
                        msg.MarkPublished();
                    }
                    catch (Exception ex)
                    {
                        msg.MarkFailed(ex.Message);
                        _logger.LogError(ex, "Outbox publish failed for {MessageId}", msg.Id);
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OutboxPublisher loop error");
                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private static string ResolveQueueName(string eventType) => eventType switch
    {
        "PersonUpserted" => "contact-events",
        "ContactInfoUpserted" => "contact-events",
        "ContactInfoDeleted" => "contact-events",
        _ => "contact-events"
    };
}


