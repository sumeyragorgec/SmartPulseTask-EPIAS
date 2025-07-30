using Microsoft.AspNetCore.Mvc;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;
using SmartPulseTask.Infrastructure.Services;

namespace SmartPulseTask.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChartController : ControllerBase
{
    private readonly IEpiasDataService _epiasDataService;
    private readonly IChartDataService _chartDataService;
    private readonly ILogger<ChartController> _logger;

    public ChartController(
            IEpiasDataService epiasDataService,
            IChartDataService chartDataService,
            ILogger<ChartController> logger)
    {
        _epiasDataService = epiasDataService;
        _chartDataService = chartDataService;
        _logger = logger;
    }

    [HttpGet("hourly-analysis")]
    public async Task<IActionResult> GetHourlyAnalysis(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(1));
            var dateRange = new DateRange(startDate, endDate);

            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var chartData = _chartDataService.GetHourlyAnalysis(transactions);

            return Ok(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("contract-analysis")]
    public async Task<IActionResult> GetContractAnalysis(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(1));
            var dateRange = new DateRange(startDate, endDate);

            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var chartData = _chartDataService.GetContractAnalysis(transactions);

            return Ok(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("price-trend")]
    public async Task<IActionResult> GetPriceTrend(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(1));
            var dateRange = new DateRange(startDate, endDate);

            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var chartData = _chartDataService.GetPriceTrend(transactions);

            return Ok(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("volume-analysis")]
    public async Task<IActionResult> GetVolumeAnalysis(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(1));
            var dateRange = new DateRange(startDate, endDate);

            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var chartData = _chartDataService.GetVolumeAnalysis(transactions);

            return Ok(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("summary-stats")]
    public async Task<IActionResult> GetSummaryStats(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(1));
            var dateRange = new DateRange(startDate, endDate);

            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var chartData = _chartDataService.GetSummaryStats(transactions);

            return Ok(new { success = true, data = chartData });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }

    [HttpGet("all-charts")]
    public async Task<IActionResult> GetAllChartData(
        [FromQuery] string tgtToken,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            var token = new TgtToken(tgtToken, TimeSpan.FromHours(2));
            var dateRange = new DateRange(startDate, endDate); 
            var transactions = await _epiasDataService.GetTransactionHistoryAsync(token, dateRange);
            var transactionList = transactions.ToList();  
            var chartTasks = new[]
            {
            Task.Run(() => new { key = "summary", data = _chartDataService.GetSummaryStats(transactionList) }),
            Task.Run(() => new { key = "hourly", data = _chartDataService.GetHourlyAnalysis(transactionList) }),
            Task.Run(() => new { key = "contract", data = _chartDataService.GetContractAnalysis(transactionList) }),
            Task.Run(() => new { key = "price", data = _chartDataService.GetPriceTrend(transactionList) }),
            Task.Run(() => new { key = "volume", data = _chartDataService.GetVolumeAnalysis(transactionList) })
        };

            var results = await Task.WhenAll(chartTasks);

            var response = new
            {
                success = true,
                summary = results[0].data,
                hourlyAnalysis = results[1].data,
                contractAnalysis = results[2].data,
                priceTrend = results[3].data,
                volumeAnalysis = results[4].data
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, error = ex.Message });
        }
    }
}