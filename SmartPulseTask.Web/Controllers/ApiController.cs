using Microsoft.AspNetCore.Mvc;
using SmartPulseTask.Application.UseCases;
using SmartPulseTask.Domain.ValueObjects;
using SmartPulseTask.Web.Models;

namespace SmartPulseTask.Web.Controllers;


[ApiController]
[Route("api/[controller]")]
public class ApiController : ControllerBase
{
    private readonly IAnalyzeTransactionsUseCase _analyzeTransactionsUseCase;
    private readonly IGetTgtTokenUseCase _getTgtTokenUseCase;
    private readonly ILogger<ApiController> _logger;

    public ApiController(
           IAnalyzeTransactionsUseCase analyzeTransactionsUseCase,
           IGetTgtTokenUseCase getTgtTokenUseCase,
           ILogger<ApiController> logger)
    {
        _analyzeTransactionsUseCase = analyzeTransactionsUseCase ;
        _getTgtTokenUseCase = getTgtTokenUseCase ;
        _logger = logger;
    }

    [HttpPost("authenticate")]
    public async Task<ActionResult<ApiResponseViewModel<TgtTokenViewModel>>> Authenticate([FromBody] LoginViewModel model)
    {
        try
        {
            var credentials = new UserCredentials(model.Username, model.Password);
            var tgtToken = await _getTgtTokenUseCase.ExecuteAsync(credentials);

            var tokenViewModel = new TgtTokenViewModel
            {
                Value = tgtToken.Value,
                CreatedAt = tgtToken.CreatedAt,
                ExpiresAt = tgtToken.ExpiresAt,
                IsExpired = tgtToken.IsExpired,
                TimeRemaining = tgtToken.IsExpired ? TimeSpan.Zero : tgtToken.ExpiresAt - DateTime.UtcNow
            };

            return Ok(ApiResponseViewModel<TgtTokenViewModel>.SuccessResult(tokenViewModel));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseViewModel<TgtTokenViewModel>.ErrorResult(ex.Message));
        }
    }

    [HttpPost("analyze")]
    public async Task<ActionResult<ApiResponseViewModel<List<ContractAnalysisResultViewModel>>>> AnalyzeTransactions(
        [FromBody] TransactionAnalysisRequest request)
    {
        try
        {
            var credentials = new UserCredentials(request.Username, request.Password);
            var dateRange = new DateRange(request.StartDate, request.EndDate);

            var results = await _analyzeTransactionsUseCase.ExecuteAsync(credentials, dateRange);

            var resultViewModels = results.Select(r => new ContractAnalysisResultViewModel
            {
                ContractName = r.ContractName,
                ContractDateTime = r.ContractDateTime,
                TotalAmount = r.TotalAmount,
                TotalQuantity = r.TotalQuantity,
                WeightedAveragePrice = r.WeightedAveragePrice,
                TransactionCount = r.TransactionCount
            }).ToList();

            return Ok(ApiResponseViewModel<List<ContractAnalysisResultViewModel>>.SuccessResult(resultViewModels));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponseViewModel<List<ContractAnalysisResultViewModel>>.ErrorResult(ex.Message));
        }
    }
}
