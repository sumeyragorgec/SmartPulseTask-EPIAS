using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public class GetTgtTokenUseCase : IGetTgtTokenUseCase
{
    private readonly IEpiasAuthenticationService _authService;
    private readonly ITgtTokenCache _tokenCache;

    public GetTgtTokenUseCase(
        IEpiasAuthenticationService authService,
        ITgtTokenCache tokenCache)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
    }

    public async Task<TgtToken> ExecuteAsync(UserCredentials credentials)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        var cachedToken = await _tokenCache.GetCachedTokenAsync();

        if (cachedToken != null && !cachedToken.IsExpired)
            return cachedToken;
        var newToken = await _authService.GetTgtTokenAsync(credentials);
        await _tokenCache.SetCachedTokenAsync(newToken);

        return newToken;
    }
}
