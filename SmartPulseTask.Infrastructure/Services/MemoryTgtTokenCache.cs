using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Infrastructure.Services;


public class MemoryTgtTokenCache : ITgtTokenCache
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryTgtTokenCache> _logger;
    private const string CACHE_KEY = "TgtToken";

    public MemoryTgtTokenCache(
        IMemoryCache memoryCache,
        ILogger<MemoryTgtTokenCache> logger)
    {
        _memoryCache = memoryCache ;
        _logger = logger;
    }

    public async Task<TgtToken?> GetCachedTokenAsync()
    {
        if (_memoryCache.TryGetValue(CACHE_KEY, out TgtToken? cachedToken))
        {
            if (cachedToken != null && !cachedToken.IsExpired)
            { 
                return cachedToken;
            }
            else
            { 
                await InvalidateCacheAsync();
            }
        } 
        return null;
    }

    public async Task SetCachedTokenAsync(TgtToken token)
    {
        if (token == null)
            throw new ArgumentNullException(nameof(token));

        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = token.ExpiresAt,
            Priority = CacheItemPriority.High
        };

        _memoryCache.Set(CACHE_KEY, token, cacheOptions); 

        await Task.CompletedTask;
    }

    public async Task InvalidateCacheAsync()
    {
        _memoryCache.Remove(CACHE_KEY); 

        await Task.CompletedTask;
    }
}
