using PhoneRegistry.Messaging.Models;
using PhoneRegistry.Messaging.Services;
using PhoneRegistry.WorkerService.Services;

namespace PhoneRegistry.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(
        ILogger<Worker> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Report Processing Worker started");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var reportConsumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer<ReportRequestMessage>>();
            var contactEventsConsumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer<IntegrationEventEnvelope>>();
            
            // RabbitMQ consumer'ı başlat
            reportConsumer.StartConsuming("report-processing-queue");
            contactEventsConsumer.StartConsuming("contact-events");

            // Worker çalışmaya devam etsin
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken); // 5 saniyede bir health check
            }
            
            // Temizlik
            reportConsumer.StopConsuming();
            contactEventsConsumer.StopConsuming();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Report Processing Worker");
            throw;
        }
        finally
        {
            _logger.LogInformation("Report Processing Worker stopped");
        }
    }

    public override void Dispose()
    {
        base.Dispose();
    }
}
