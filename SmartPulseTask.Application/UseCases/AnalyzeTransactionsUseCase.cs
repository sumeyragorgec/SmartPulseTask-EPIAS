using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public class AnalyzeTransactionsUseCase : IAnalyzeTransactionsUseCase
{
    private readonly IEpiasAuthenticationService _authService;
    private readonly IEpiasDataService _dataService;
    private readonly ITransactionAnalysisService _analysisService;
    private readonly ITgtTokenCache _tokenCache;

    public AnalyzeTransactionsUseCase(
        IEpiasAuthenticationService authService,
        IEpiasDataService dataService,
        ITransactionAnalysisService analysisService,
        ITgtTokenCache tokenCache)
    {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _analysisService = analysisService ?? throw new ArgumentNullException(nameof(analysisService));
        _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
    }

    public async Task<IEnumerable<ContractAnalysisResult>> ExecuteAsync(
        UserCredentials credentials,
        DateRange dateRange)
    {
        if (credentials is null)
            throw new ArgumentNullException(nameof(credentials));

        if (dateRange is null)
            throw new ArgumentNullException(nameof(dateRange));

        var tgtToken = await _tokenCache.GetCachedTokenAsync();

        if (tgtToken is null || tgtToken.IsExpired)
        {
            tgtToken = await _authService.GetTgtTokenAsync(credentials);
            await _tokenCache.SetCachedTokenAsync(tgtToken);
        }

        var transactions = await _dataService.GetTransactionHistoryAsync(tgtToken, dateRange);
        var results = _analysisService.AnalyzeTransactions(transactions);

        return results;
    }
}
