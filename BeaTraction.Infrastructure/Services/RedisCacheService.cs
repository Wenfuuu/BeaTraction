using System.Text.Json;
using BeaTraction.Application.Interfaces;
using StackExchange.Redis;

namespace BeaTraction.Infrastructure.Services;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly IDatabase? _db;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IConnectionMultiplexer? redis)
    {
        _redis = redis;
        _db = redis?.GetDatabase();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
        
        if (_redis != null && _redis.IsConnected)
        {
            Console.WriteLine("RedisCacheService: Redis is connected and ready");
        }
        else if (_redis != null && !_redis.IsConnected)
        {
            Console.WriteLine("RedisCacheService: Redis multiplexer exists but not connected, will retry automatically");
        }
        else
        {
            Console.WriteLine("RedisCacheService: No Redis connection available, running in fallback mode");
        }
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            if (_redis == null || _db == null || !_redis.IsConnected)
            {
                return null;
            }

            var value = await _db.StringGetAsync(key);
            
            if (value.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis connection error on GET: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis error on GET: {ex.Message}");
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (_redis == null || _db == null || !_redis.IsConnected)
            {
                return;
            }

            var serialized = JsonSerializer.Serialize(value, _jsonOptions);
            await _db.StringSetAsync(key, serialized, expiration);
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis connection error on SET: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis error on SET: {ex.Message}");
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            if (_redis == null || _db == null || !_redis.IsConnected)
            {
                return;
            }

            await _db.KeyDeleteAsync(key);
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis connection error on REMOVE: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis error on REMOVE: {ex.Message}");
        }
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        try
        {
            if (_redis == null || _db == null || !_redis.IsConnected)
            {
                return;
            }

            var endpoints = _redis.GetEndPoints();
            if (endpoints.Length == 0)
            {
                Console.WriteLine("No Redis endpoints available");
                return;
            }

            var server = _redis.GetServer(endpoints.First());
            
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            
            if (keys.Length > 0)
            {
                await _db.KeyDeleteAsync(keys);
                Console.WriteLine($"Invalidated {keys.Length} cache keys with prefix: {prefix}");
            }
        }
        catch (RedisConnectionException ex)
        {
            Console.WriteLine($"Redis connection error on REMOVE_BY_PREFIX: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Redis error on REMOVE_BY_PREFIX: {ex.Message}");
        }
    }
}
