using System.Net;
using System.Text.Json;
using ChakChakShop.API.DTO.Responses;

namespace ChakChakShop.API.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly Dictionary<string, RateLimitInfo> _rateLimitStore = new();
    private readonly int _maxRequests = 100;
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"{clientIp}:{context.Request.Path}";

        if (!_rateLimitStore.ContainsKey(key))
        {
            _rateLimitStore[key] = new RateLimitInfo
            {
                RequestCount = 0,
                ResetTime = DateTime.UtcNow.Add(_timeWindow)
            };
        }

        var rateLimitInfo = _rateLimitStore[key];

        if (DateTime.UtcNow > rateLimitInfo.ResetTime)
        {
            rateLimitInfo.RequestCount = 0;
            rateLimitInfo.ResetTime = DateTime.UtcNow.Add(_timeWindow);
        }

        if (rateLimitInfo.RequestCount >= _maxRequests)
        {
            _logger.LogWarning("Rate limit exceeded for {ClientIp} on {Path}", clientIp, context.Request.Path);
            
            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.TooManyRequests,
                Message = "Rate limit exceeded. Please try again later.",
                Timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
            return;
        }

        rateLimitInfo.RequestCount++;
        await _next(context);
    }

    private class RateLimitInfo
    {
        public int RequestCount { get; set; }
        public DateTime ResetTime { get; set; }
    }
}


