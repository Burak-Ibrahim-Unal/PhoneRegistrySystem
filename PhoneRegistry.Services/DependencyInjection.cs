using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Application;
using PhoneRegistry.Services.Interfaces;
using PhoneRegistry.Services.Implementations;

namespace PhoneRegistry.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // Application katmanını ekle (MediatR burada configure edilir)
        services.AddApplication();

        // MediatR'ı Services katmanında kullanmak için
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Business Services
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
