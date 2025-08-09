using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace PhoneRegistry.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // AutoMapper
        services.AddAutoMapper(assembly);

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        // MediatR burada değil, Services katmanında configure edilecek
        // Sadece Application katmanındaki handler'lar için MediatR gerekli
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        return services;
    }
}
