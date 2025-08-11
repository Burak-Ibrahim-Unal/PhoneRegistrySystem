using PhoneRegistry.Services;
using PhoneRegistry.Infrastructure;
using PhoneRegistry.WorkerService;
// Worker ve RabbitMQ kayıtları bu API'den kaldırıldı (microservice ayrımı)
using PhoneRegistry.Infrastructure.Data;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using PhoneRegistry.Infrastructure.Repositories;
using PhoneRegistry.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Caching;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddServices(); // Bu Application + MediatR'ı da içerir
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCaching(builder.Configuration); // Redis cache

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(tp => tp
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PhoneRegistry.ContactApi"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

// Not: Worker ayrı süreçte çalışır (PhoneRegistry.WorkerService)

// CORS - Daha kapsamlı ayar
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
    
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4300", "http://127.0.0.1:4300", "https://localhost:4300", "https://127.0.0.1:4300")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Register Outbox publisher background service (Build'ten ÖNCE)
builder.Services.AddScoped<OutboxRepository>();
builder.Services.AddHostedService<OutboxPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(); // Default policy kullan
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Auto-migrate and Seed default data on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Apply migrations for both contexts
    var contactDb = services.GetRequiredService<ContactDbContext>();
    await contactDb.Database.MigrateAsync();
    var reportDb = services.GetRequiredService<ReportDbContext>();
    await reportDb.Database.MigrateAsync();

    // Seed combined context data
    var context = services.GetRequiredService<PhoneRegistryDbContext>();
    var logger = services.GetService<ILoggerFactory>()?.CreateLogger("DbSeeder");
    await DbSeeder.SeedCitiesAsync(context, logger);
    await DbSeeder.SeedDemoDataAsync(context, logger);
    await DbSeeder.SeedFixedReportAsync(context, logger);
}

app.Run();
