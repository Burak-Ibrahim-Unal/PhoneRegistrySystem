using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;
using PhoneRegistry.Infrastructure.Repositories;
using PhoneRegistry.Messaging.Interfaces;
using PhoneRegistry.Messaging.Services;
using StackExchange.Redis;
using PhoneRegistry.Infrastructure.Services;
using PhoneRegistry.Application.Common.Interfaces;
using PhoneRegistry.Caching;

namespace PhoneRegistry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Databases: split Contact and Report contexts (same DB, different schemas)
        services.AddDbContext<ContactDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        services.AddDbContext<ReportDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        // Composite context (for migrations/seeding across both schemas)
        services.AddDbContext<PhoneRegistryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        // Per-bounded-context UnitOfWork
        services.AddScoped<IContactUnitOfWork, ContactUnitOfWork>();
        services.AddScoped<IReportUnitOfWork, ReportUnitOfWork>();

        // Caching (Redis)
        services.AddCaching(configuration);

        // RabbitMQ
        services.AddSingleton<RabbitMQConnectionService>();
        services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

        // Outbox services
        services.AddScoped<IOutboxWriter, OutboxWriter>();

        return services;
    }
}
