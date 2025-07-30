using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Infrastructure.Services;

public class InMemoryTgtTokenCache : ITgtTokenCache
{
    private TgtToken? _cachedToken;

    public Task<TgtToken?> GetCachedTokenAsync()
    {
        return Task.FromResult(_cachedToken?.IsExpired == false ? _cachedToken : null);
    }

    public Task SetCachedTokenAsync(TgtToken token)
    {
        _cachedToken = token ?? throw new ArgumentNullException(nameof(token));
        return Task.CompletedTask;
    }

    public Task InvalidateCacheAsync()
    {
        _cachedToken = null;
        return Task.CompletedTask;
    }
}
