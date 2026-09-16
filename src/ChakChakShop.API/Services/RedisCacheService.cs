using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace ChakChakShop.API.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (!_database.Multiplexer.IsConnected)
            {
                _logger.LogWarning("Redis is not connected, skipping cache get for key {Key}", key);
                return null;
            }
            
            var value = await _database.StringGetAsync(key);
            if (!value.HasValue)
            {
                return null;
            }

            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cache key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (!_database.Multiplexer.IsConnected)
            {
                _logger.LogWarning("Redis is not connected, skipping cache set for key {Key}", key);
                return;
            }
            
            var json = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, json, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache key {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_database.Multiplexer.IsConnected)
            {
                _logger.LogWarning("Redis is not connected, skipping cache remove for key {Key}", key);
                return;
            }
            
            await _database.KeyDeleteAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_database.Multiplexer.IsConnected)
            {
                _logger.LogWarning("Redis is not connected, skipping cache pattern remove for {Pattern}", pattern);
                return;
            }
            
            var endpoints = _database.Multiplexer.GetEndPoints();
            if (endpoints == null || !endpoints.Any())
            {
                _logger.LogWarning("No Redis endpoints available, skipping cache pattern remove for {Pattern}", pattern);
                return;
            }
            
            var server = _database.Multiplexer.GetServer(endpoints.First());
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache pattern {Pattern}", pattern);
        }
    }
}


