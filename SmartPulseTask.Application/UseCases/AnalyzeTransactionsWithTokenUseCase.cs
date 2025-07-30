using Microsoft.Extensions.Logging;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public class AnalyzeTransactionsWithTokenUseCase : IAnalyzeTransactionsWithTokenUseCase
{
    private readonly IEpiasDataService _dataService;
    private readonly ITransactionAnalysisService _analysisService;
    private readonly ILogger<AnalyzeTransactionsWithTokenUseCase> _logger;

    public AnalyzeTransactionsWithTokenUseCase(
        IEpiasDataService dataService,
        ITransactionAnalysisService analysisService,
        ILogger<AnalyzeTransactionsWithTokenUseCase> logger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ContractAnalysisResult>> ExecuteAsync(
        TgtToken tgtToken,
        DateRange dateRange)
    {
        if (tgtToken is null)
            throw new ArgumentNullException(nameof(tgtToken));

        if (dateRange is null)
            throw new ArgumentNullException(nameof(dateRange));

        if (tgtToken.IsExpired)
        { 
            throw new ArgumentException("expired TGT token");
        }
        var transactions = await _dataService.GetTransactionHistoryAsync(tgtToken, dateRange);

        var results = _analysisService.AnalyzeTransactions(transactions);

        return results;
    }
}
