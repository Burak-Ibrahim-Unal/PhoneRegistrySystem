using PhoneRegistry.Infrastructure;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Models;
using PhoneRegistry.Messaging.Services;
using PhoneRegistry.WorkerService;
using PhoneRegistry.WorkerService.Services;
using Serilog;

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// Serilog
builder.Services.AddSerilog();

// Infrastructure services (Database, RabbitMQ, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Message processing services
builder.Services.AddScoped<IMessageConsumer<ReportRequestMessage>, ReportProcessingService>();
builder.Services.AddScoped<RabbitMQConsumer<ReportRequestMessage>>();

// Worker service
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

try
{
    Log.Information("Starting Report Processing Worker Service");
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Report Processing Worker Service failed to start");
}
finally
{
    Log.CloseAndFlush();
}
