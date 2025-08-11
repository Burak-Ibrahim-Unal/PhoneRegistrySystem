using PhoneRegistry.Services;
using PhoneRegistry.Infrastructure;
using Serilog;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddOpenTelemetry()
    .WithTracing(tp => tp
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PhoneRegistry.ReportApi"))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

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

app.Run();