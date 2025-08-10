using PhoneRegistry.Services;
using PhoneRegistry.Infrastructure;
using PhoneRegistry.WorkerService;
using PhoneRegistry.WorkerService.Services;
using PhoneRegistry.Application.Common.Messaging;
using PhoneRegistry.Infrastructure.Messaging.Interfaces;
using PhoneRegistry.Infrastructure.Messaging.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddServices(); // Bu Application + MediatR'ı da içerir
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Worker Service için gerekli servisleri ekle
builder.Services.AddScoped<IMessageConsumer<ReportRequestMessage>, ReportProcessingService>();
builder.Services.AddScoped<RabbitMQConsumer<ReportRequestMessage>>();

// Worker Service'i de ekle
builder.Services.AddHostedService<Worker>();

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

app.Run();
