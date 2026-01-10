using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ChakChakShop.API.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;

    public RedisHealthCheck(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_redis.IsConnected)
            {
                var endpoints = _redis.GetEndPoints();
                if (endpoints == null || !endpoints.Any())
                {
                    return HealthCheckResult.Unhealthy("Redis is not configured");
                }
                return HealthCheckResult.Degraded("Redis is not connected but multiplexer is initialized");
            }
            
            var database = _redis.GetDatabase();
            await database.StringGetAsync("health_check");
            
            return HealthCheckResult.Healthy("Redis is available");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis is unavailable", ex);
        }
    }
}


