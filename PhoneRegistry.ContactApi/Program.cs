using PhoneRegistry.Services;
using PhoneRegistry.Infrastructure;
using PhoneRegistry.WorkerService;
// Worker ve RabbitMQ kayıtları bu API'den kaldırıldı (microservice ayrımı)
using PhoneRegistry.Infrastructure.Data;
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

// Seed default cities on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PhoneRegistryDbContext>();
    var logger = services.GetService<ILoggerFactory>()?.CreateLogger("DbSeeder");
    await DbSeeder.SeedCitiesAsync(context, logger);
    await DbSeeder.SeedDemoDataAsync(context, logger);
    await DbSeeder.SeedFixedReportAsync(context, logger);
}

app.Run();
