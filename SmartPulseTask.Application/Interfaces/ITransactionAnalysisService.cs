using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Application.Interfaces;

public interface ITransactionAnalysisService
{
    IEnumerable<ContractAnalysisResult> AnalyzeTransactions(
            IEnumerable<TransactionHistoryGipData> transactions);
}