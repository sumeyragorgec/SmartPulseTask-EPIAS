using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Web.Services;

public class SessionService : ISessionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<SessionService> _logger;

    private const string TGT_TOKEN_KEY = "TgtToken";
    private const string TGT_CREATED_KEY = "TgtCreatedAt";
    private const string TGT_EXPIRES_KEY = "TgtExpiresAt";
    private const string USERNAME_KEY = "Username";

    public SessionService(IHttpContextAccessor httpContextAccessor, ILogger<SessionService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private ISession Session => _httpContextAccessor.HttpContext?.Session
        ?? throw new InvalidOperationException("Session is not available");

    public async Task<TgtToken?> GetCurrentTokenAsync()
    {
        try
        {
            var tokenValue = Session.GetString(TGT_TOKEN_KEY);
            var createdAtStr = Session.GetString(TGT_CREATED_KEY);
            var expiresAtStr = Session.GetString(TGT_EXPIRES_KEY);

            if (string.IsNullOrEmpty(tokenValue) ||
                string.IsNullOrEmpty(createdAtStr) ||
                string.IsNullOrEmpty(expiresAtStr))
            {
                return null;
            }

            if (!DateTime.TryParse(createdAtStr, out var createdAt) ||
                !DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                _logger.LogWarning("Invalid date format in session token data");
                await ClearSessionAsync();
                return null;
            }

            if (DateTime.UtcNow >= expiresAt)
            {
                _logger.LogInformation("Token expired, clearing session");
                await ClearSessionAsync();
                return null;
            }
            var validityDuration = expiresAt - createdAt;
            var token = new TgtToken(tokenValue, validityDuration);

            return token;
        }
        catch (Exception ex)
        {
            await ClearSessionAsync();
            return null;
        }
    }

    public async Task SetTokenAsync(TgtToken token, string username)
    {
        if (token == null) throw new ArgumentNullException(nameof(token));
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));

        try
        {
            Session.SetString(TGT_TOKEN_KEY, token.Value);
            Session.SetString(TGT_CREATED_KEY, token.CreatedAt.ToString("O"));
            Session.SetString(TGT_EXPIRES_KEY, token.ExpiresAt.ToString("O"));
            Session.SetString(USERNAME_KEY, username);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task ClearSessionAsync()
    {
        try
        {
            var username = Session.GetString(USERNAME_KEY);
            Session.Clear();

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
        }
    }

    public bool IsValidSession()
    {
        try
        {
            var tokenValue = Session.GetString(TGT_TOKEN_KEY);
            var expiresAtStr = Session.GetString(TGT_EXPIRES_KEY);

            if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(expiresAtStr))
            {
                return false;
            }

            if (DateTime.TryParse(expiresAtStr, out var expiresAt))
            {
                return DateTime.UtcNow < expiresAt;
            }

            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public string? GetCurrentUsername()
    {
        try
        {
            return Session.GetString(USERNAME_KEY);
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}