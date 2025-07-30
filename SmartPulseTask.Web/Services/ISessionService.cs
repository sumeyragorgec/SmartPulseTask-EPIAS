using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Web.Services;

public interface ISessionService
{
    Task<TgtToken?> GetCurrentTokenAsync();
    Task SetTokenAsync(TgtToken token, string username);
    Task ClearSessionAsync();
    bool IsValidSession();
    string? GetCurrentUsername();
}
