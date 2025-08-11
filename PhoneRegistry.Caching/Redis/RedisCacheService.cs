using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhoneRegistry.Caching.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace PhoneRegistry.Caching.Redis;

public class RedisOptions
{
    public string ConnectionString { get; set; } = "localhost:6379";
    public int Database { get; set; } = 0;
}

public class RedisCacheService : ICacheService
{
    private readonly ILogger<RedisCacheService> _logger;
    private readonly ConnectionMultiplexer _mux;
    private readonly IDatabase _db;

    public RedisCacheService(IOptions<RedisOptions> options, ILogger<RedisCacheService> logger)
    {
        _logger = logger;
        _mux = ConnectionMultiplexer.Connect(options.Value.ConnectionString);
        _db = _mux.GetDatabase(options.Value.Database);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        if (!value.HasValue) return default;
        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, ttl);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
        => _db.KeyDeleteAsync(key);
}


