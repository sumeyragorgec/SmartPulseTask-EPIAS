using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Application.Interfaces;
public interface ITgtTokenCache
{
    Task<TgtToken?> GetCachedTokenAsync();
    Task SetCachedTokenAsync(TgtToken token);
    Task InvalidateCacheAsync();
}
