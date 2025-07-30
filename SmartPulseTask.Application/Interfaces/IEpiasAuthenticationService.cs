using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.Interfaces;
public interface IEpiasAuthenticationService
{
    Task<TgtToken> GetTgtTokenAsync(UserCredentials credentials);
}