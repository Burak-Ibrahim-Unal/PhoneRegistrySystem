using PhoneRegistry.Infrastructure;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Models;
using PhoneRegistry.Messaging.Services;
using PhoneRegistry.WorkerService;
using PhoneRegistry.WorkerService.Services;
using Serilog;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using Polly;
using Polly.Extensions.Http;
using System.Net;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// Serilog
builder.Services.AddSerilog();

// Infrastructure services (Database, RabbitMQ, etc.)
builder.Services.AddInfrastructure(builder.Configuration);

// Polly policies
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(new[]
    {
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(500),
        TimeSpan.FromSeconds(1)
    });

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy() => HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

builder.Services.AddHttpClient("contact-api", client =>
    {
        var baseUrl = builder.Configuration["ContactApi:BaseUrl"] ?? "http://localhost:5297";
        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// OpenTelemetry Tracing (console exporter for dev)
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PhoneRegistry.WorkerService"))
        .AddHttpClientInstrumentation()
        .AddSource("PhoneRegistry.WorkerService")
        .AddConsoleExporter());

// Message processing services
builder.Services.AddScoped<IMessageConsumer<ReportRequestMessage>, ReportProcessingService>();
builder.Services.AddScoped<RabbitMQConsumer<ReportRequestMessage>>();
builder.Services.AddScoped<IMessageConsumer<IntegrationEventEnvelope>, ContactEventsConsumer>();
builder.Services.AddScoped<RabbitMQConsumer<IntegrationEventEnvelope>>();

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
