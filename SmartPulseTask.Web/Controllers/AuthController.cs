using Microsoft.AspNetCore.Mvc;
using SmartPulseTask.Application.UseCases;
using SmartPulseTask.Domain.ValueObjects;
using SmartPulseTask.Web.Models;
namespace SmartPulseTask.Web.Controllers;
public class AuthController : Controller
{
    private readonly IGetTgtTokenUseCase _getTgtTokenUseCase;

    public AuthController(IGetTgtTokenUseCase getTgtTokenUseCase)
    {
        _getTgtTokenUseCase = getTgtTokenUseCase;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var credentials = new UserCredentials(model.Username, model.Password);
            var tgtToken = await _getTgtTokenUseCase.ExecuteAsync(credentials);

            HttpContext.Session.SetString("TgtToken", tgtToken.Value);
            HttpContext.Session.SetString("TgtCreatedAt", tgtToken.CreatedAt.ToString("O"));
            HttpContext.Session.SetString("TgtExpiresAt", tgtToken.ExpiresAt.ToString("O"));
            HttpContext.Session.SetString("Username", model.Username);

            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Giriş başarısız. Kullanıcı adı ve şifreyi kontrol ediniz.");
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult TokenInfo()
    {
        var tgtToken = HttpContext.Session.GetString("TgtToken");
        var createdAtStr = HttpContext.Session.GetString("TgtCreatedAt");
        var expiresAtStr = HttpContext.Session.GetString("TgtExpiresAt");

        if (string.IsNullOrEmpty(tgtToken) || string.IsNullOrEmpty(createdAtStr) || string.IsNullOrEmpty(expiresAtStr))
        {
            return Json(new { success = false, message = "Token bulunamadı" });
        }

        var createdAt = DateTime.Parse(createdAtStr);
        var expiresAt = DateTime.Parse(expiresAtStr);
        var isExpired = DateTime.UtcNow >= expiresAt;
        var timeRemaining = isExpired ? TimeSpan.Zero : expiresAt - DateTime.UtcNow;

        var tokenViewModel = new TgtTokenViewModel
        {
            Value = tgtToken,
            CreatedAt = createdAt,
            ExpiresAt = expiresAt,
            IsExpired = isExpired,
            TimeRemaining = timeRemaining
        };

        return Json(new { success = true, data = tokenViewModel });
    }
}
