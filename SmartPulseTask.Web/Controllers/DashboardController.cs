using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;
using SmartPulseTask.Infrastructure.Services;
using SmartPulseTask.Web.Models;
using SmartPulseTask.Web.Services;
using System.Security.Cryptography;
using System.Text;

namespace SmartPulseTask.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ISessionService _sessionService;
    private readonly IEpiasDataService _dataService;
    private readonly ITransactionAnalysisService _analysisService;
    private readonly ITgtTokenCache _tokenCache;
    private readonly ILogger<DashboardController> _logger;
    private readonly IChartDataService _chartDataService;
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan TokenValidityDuration = TimeSpan.FromHours(2);
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);
    private const string CACHE_KEY_PREFIX = "chart_data_";

    public DashboardController(
        ISessionService sessionService,
        IEpiasDataService dataService,
        ITransactionAnalysisService analysisService,
        ITgtTokenCache tokenCache,
        ILogger<DashboardController> logger,
        IChartDataService chartDataService,
        IMemoryCache memoryCache)
    {
        _sessionService = sessionService ;
        _dataService = dataService ;
        _analysisService = analysisService ;
        _tokenCache = tokenCache ;
        _logger = logger ;
        _chartDataService = chartDataService ;
        _memoryCache = memoryCache ;
    }

    public IActionResult Index()
    {
        if (!IsAuthenticated())
        {
            return RedirectToAction("Login", "Auth");
        }

        var viewModel = CreateDashboardViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AnalyzeTransactions(TransactionAnalysisViewModel model)
    {
        var validationResult = await ValidateRequestAsync(model);
        if (!validationResult.IsValid)
        {
            return Json(validationResult.ErrorResponse);
        }

        try
        {
            var (tgtToken, dateRange) = CreateRequestParameters(validationResult.TokenValue!, model);
             

            var transactions = await _dataService.GetTransactionHistoryAsync(tgtToken, dateRange);

            if (!transactions.Any())
            {
                return Json(ApiResponseViewModel<List<ContractAnalysisResultViewModel>>.SuccessResult(
                    new List<ContractAnalysisResultViewModel>()));
            }

            var results = _analysisService.AnalyzeTransactions(transactions);
            var resultViewModels = MapToViewModels(results);

            return Json(ApiResponseViewModel<List<ContractAnalysisResultViewModel>>.SuccessResult(resultViewModels));
        }
        catch (HttpRequestException ex)
        {
            return HandleHttpException(ex);
        }
        catch (ArgumentException ex)
        {
            return Json(ApiResponseViewModel<object>.ErrorResult($"Geçersiz parametre: {ex.Message}"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during transaction analysis");
            return Json(ApiResponseViewModel<object>.ErrorResult(
                "İşlem analizi sırasında beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz."));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetChartData(DateTime startDate, DateTime endDate)
    {
        var validationResult = ValidateDateRange(startDate, endDate);
        if (!validationResult.IsValid)
        {
            return Json(new { success = false, errorMessage = validationResult.ErrorMessage });
        }

        if (!IsAuthenticated())
        {
            return Json(new { success = false, errorMessage = "Oturum süresi dolmuş. Lütfen yeniden giriş yapın." });
        }

        try
        {
            var cacheKey = GenerateCacheKey(startDate, endDate);
            if (_memoryCache.TryGetValue(cacheKey, out object? cachedData))
            {
                _logger.LogInformation("Returning cached chart data for key: {CacheKey}", cacheKey);
                return Json(cachedData);
            }

            var (tgtToken, dateRange) = await GetTokenAndDateRangeAsync(startDate, endDate);
            if (tgtToken == null)
            {
                return Json(new { success = false, errorMessage = "Token bulunamadı. Lütfen yeniden giriş yapın." });
            }


            var transactions = await _dataService.GetTransactionHistoryAsync(tgtToken, dateRange);
            var transactionList = transactions.ToList();

            if (!transactionList.Any())
            {
                var emptyResult = new { success = false, errorMessage = "Belirtilen tarih aralığında işlem bulunamadı." };
                return Json(emptyResult);
            }

            var chartData = await ProcessChartDataAsync(transactionList);

            _memoryCache.Set(cacheKey, chartData, CacheExpiration);

            _logger.LogInformation("Successfully processed {TransactionCount} transactions for chart data",
                transactionList.Count);

            return Json(chartData);
        }
        catch (HttpRequestException ex)
        {
            return HandleHttpException(ex, isJsonResponse: true);
        }
        catch (Exception ex)
        {
            return Json(new { success = false, errorMessage = "Grafik verileri işlenirken hata oluştu." });
        }
    }

    #region Private Helper Methods

    private DashboardViewModel CreateDashboardViewModel()
    {
        var viewModel = new DashboardViewModel();
        var (tgtToken, createdAtStr, expiresAtStr) = GetSessionTokenInfo();

        if (!string.IsNullOrEmpty(tgtToken) &&
            !string.IsNullOrEmpty(createdAtStr) &&
            !string.IsNullOrEmpty(expiresAtStr) &&
            DateTime.TryParse(createdAtStr, out var createdAt) &&
            DateTime.TryParse(expiresAtStr, out var expiresAt))
        {
            viewModel.CurrentToken = new TgtTokenViewModel
            {
                Value = tgtToken,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                IsExpired = DateTime.UtcNow >= expiresAt,
                TimeRemaining = DateTime.UtcNow >= expiresAt ? TimeSpan.Zero : expiresAt - DateTime.UtcNow
            };
        }

        return viewModel;
    }

    private async Task<ValidationResult> ValidateRequestAsync(TransactionAnalysisViewModel model)
    {
        var token = await _sessionService.GetCurrentTokenAsync();
        if (token == null)
        {
            return ValidationResult.Invalid(ApiResponseViewModel<object>.ErrorResult("Token bulunamadı."));
        }

        if (!IsAuthenticated())
        {
            return ValidationResult.Invalid(ApiResponseViewModel<object>.ErrorResult("Oturum süresi dolmuş. Lütfen yeniden giriş yapın."));
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return ValidationResult.Invalid(ApiResponseViewModel<object>.ValidationErrorResult(errors));
        }

        var (tokenValue, _, expiresAtStr) = GetSessionTokenInfo();

        if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(expiresAtStr))
        {
            return ValidationResult.Invalid(ApiResponseViewModel<object>.ErrorResult("Token bulunamadı. Lütfen yeniden giriş yapın."));
        }

        if (DateTime.TryParse(expiresAtStr, out var expiresAt) && DateTime.UtcNow >= expiresAt)
        {
            HttpContext.Session.Clear();
            return ValidationResult.Invalid(ApiResponseViewModel<object>.ErrorResult("Token süresi dolmuş. Lütfen yeniden giriş yapın."));
        }

        return ValidationResult.Valid(tokenValue);
    }

    private (TgtToken, DateRange) CreateRequestParameters(string tokenValue, TransactionAnalysisViewModel model)
    {
        var tgtToken = new TgtToken(tokenValue, TokenValidityDuration);
        var dateRange = new DateRange(model.StartDate, model.EndDate);
        return (tgtToken, dateRange);
    }

    private async Task<(TgtToken?, DateRange)> GetTokenAndDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var (tokenValue, _, expiresAtStr) = GetSessionTokenInfo();

        if (string.IsNullOrEmpty(tokenValue) || string.IsNullOrEmpty(expiresAtStr))
        {
            return (null, new DateRange(startDate, endDate));
        }

        if (DateTime.TryParse(expiresAtStr, out var expiresAt) && DateTime.UtcNow >= expiresAt)
        {
            HttpContext.Session.Clear();
            return (null, new DateRange(startDate, endDate));
        }

        var tgtToken = new TgtToken(tokenValue, TokenValidityDuration);
        var dateRange = new DateRange(startDate, endDate);

        return (tgtToken, dateRange);
    }

    private async Task<object> ProcessChartDataAsync(List<TransactionHistoryGipData> transactions)
    {
        var chartTasks = new Task<object>[]
        {
            Task.Run(() => _chartDataService.GetSummaryStats(transactions)),
            Task.Run(() => _chartDataService.GetHourlyAnalysis(transactions)),
            Task.Run(() => _chartDataService.GetContractAnalysis(transactions)),
            Task.Run(() => _chartDataService.GetPriceTrend(transactions)),
            Task.Run(() => _chartDataService.GetVolumeAnalysis(transactions))
        };

        var results = await Task.WhenAll(chartTasks);

        return new
        {
            success = true,
            summary = results[0],
            hourlyAnalysis = results[1],
            contractAnalysis = results[2],
            priceTrend = results[3],
            volumeAnalysis = results[4]
        };
    }

    private List<ContractAnalysisResultViewModel> MapToViewModels(IEnumerable<ContractAnalysisResult> results)
    {
        return results.Select(r => new ContractAnalysisResultViewModel
        {
            ContractName = r.ContractName,
            ContractDateTime = r.ContractDateTime,
            TotalAmount = r.TotalAmount,
            TotalQuantity = r.TotalQuantity,
            WeightedAveragePrice = r.WeightedAveragePrice,
            TransactionCount = r.TransactionCount
        }).ToList();
    }

    private JsonResult HandleHttpException(HttpRequestException ex, bool isJsonResponse = false)
    {

        if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized"))
        {
            HttpContext.Session.Clear();
            var errorMessage = "Token geçersiz. Lütfen yeniden giriş yapın.";

            return isJsonResponse
                ? Json(new { success = false, errorMessage })
                : Json(ApiResponseViewModel<object>.ErrorResult(errorMessage));
        }

        return isJsonResponse
            ? Json(new { success = false, errorMessage = "Error" })
            : Json(ApiResponseViewModel<object>.ErrorResult(""));
    }

    private bool IsAuthenticated()
    {
        var (tgtToken, _, expiresAtStr) = GetSessionTokenInfo();

        if (string.IsNullOrEmpty(tgtToken) || string.IsNullOrEmpty(expiresAtStr))
        {
            return false;
        }

        return DateTime.TryParse(expiresAtStr, out var expiresAt) && DateTime.UtcNow < expiresAt;
    }

    private (string? tgtToken, string? createdAt, string? expiresAt) GetSessionTokenInfo()
    {
        return (
            HttpContext.Session.GetString("TgtToken"),
            HttpContext.Session.GetString("TgtCreatedAt"),
            HttpContext.Session.GetString("TgtExpiresAt")
        );
    }

    private string GenerateCacheKey(DateTime startDate, DateTime endDate)
    {
        var keyString = $"{CACHE_KEY_PREFIX}{startDate:yyyyMMddHHmm}_{endDate:yyyyMMddHHmm}";

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(keyString));
        return Convert.ToBase64String(hashBytes);
    }

    private DateValidationResult ValidateDateRange(DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
        {
            return DateValidationResult.Invalid("Başlangıç tarihi bitiş tarihinden küçük olmalı.");
        }

        if (endDate > DateTime.Now)
        {
            return DateValidationResult.Invalid("Gelecek tarih seçilemez.");
        }

        var timeDiff = endDate - startDate;
        if (timeDiff.TotalDays > 90)
        {
            return DateValidationResult.Invalid("Maksimum 90 günlük veri çekilebilir.");
        }

        return DateValidationResult.Valid();
    }

    #endregion

    #region Helper Classes

    private class ValidationResult
    {
        public bool IsValid { get; init; }
        public string? TokenValue { get; init; }
        public object? ErrorResponse { get; init; }

        public static ValidationResult Valid(string tokenValue) => new() { IsValid = true, TokenValue = tokenValue };
        public static ValidationResult Invalid(object errorResponse) => new() { IsValid = false, ErrorResponse = errorResponse };
    }

    private class DateValidationResult
    {
        public bool IsValid { get; init; }
        public string? ErrorMessage { get; init; }

        public static DateValidationResult Valid() => new() { IsValid = true };
        public static DateValidationResult Invalid(string errorMessage) => new() { IsValid = false, ErrorMessage = errorMessage };
    }

    #endregion
}