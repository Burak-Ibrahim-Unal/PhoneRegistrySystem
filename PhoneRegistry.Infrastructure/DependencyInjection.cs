using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Domain.Repositories;
using PhoneRegistry.Infrastructure.Data;
using PhoneRegistry.Infrastructure.Repositories;
using PhoneRegistry.Infrastructure.Messaging.Interfaces;
using PhoneRegistry.Infrastructure.Messaging.Services;
using StackExchange.Redis;

namespace PhoneRegistry.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PhoneRegistryDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IContactInfoRepository, ContactInfoRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Redis
        services.AddSingleton<IConnectionMultiplexer>(provider =>
        {
            var connectionString = configuration.GetConnectionString("Redis");
            return ConnectionMultiplexer.Connect(connectionString!);
        });

        // RabbitMQ
        services.AddSingleton<RabbitMQConnectionService>();
        services.AddScoped<IMessagePublisher, RabbitMQPublisher>();

        return services;
    }
}
