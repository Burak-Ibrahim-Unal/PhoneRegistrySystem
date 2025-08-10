using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Infrastructure.Messaging.Services;
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
            var consumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer<ReportRequestMessage>>();
            
            // RabbitMQ consumer'ı başlat
            consumer.StartConsuming("report-processing-queue");

            // Worker çalışmaya devam etsin
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(5000, stoppingToken); // 5 saniyede bir health check
            }
            
            // Temizlik
            consumer.StopConsuming();
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
