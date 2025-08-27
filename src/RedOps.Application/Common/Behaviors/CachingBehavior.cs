using Mediator;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace RedOps.Application.Common.Behaviors;

public sealed class CachingBehavior<TMessage, TResponse> : IPipelineBehavior<TMessage, TResponse>
    where TMessage : class, IQuery<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TMessage, TResponse>> _logger;

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TMessage, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<TResponse> Handle(TMessage message, MessageHandlerDelegate<TMessage, TResponse> next, CancellationToken cancellationToken)
    {
        var cacheKey = GenerateCacheKey(message);
        
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
            return cachedResponse!;
        }

        _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);
        
        var response = await next(message, cancellationToken);

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        _cache.Set(cacheKey, response, cacheOptions);
        _logger.LogInformation("Cached response for key: {CacheKey}", cacheKey);

        return response;
    }

    private static string GenerateCacheKey(TMessage message)
    {
        var typeName = typeof(TMessage).Name;
        var serializedMessage = JsonSerializer.Serialize(message);
        var hash = serializedMessage.GetHashCode();
        return $"{typeName}:{hash}";
    }
}