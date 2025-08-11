using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhoneRegistry.Caching.Interfaces;
using PhoneRegistry.Caching.Redis;

namespace PhoneRegistry.Caching;

public static class DependencyInjection
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisOptions>(opts => configuration.GetSection("Redis").Bind(opts));
        services.AddSingleton<ICacheService, RedisCacheService>();
        return services;
    }
}


