using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public interface IGetTgtTokenUseCase
{
    Task<TgtToken> ExecuteAsync(UserCredentials credentials);
}
