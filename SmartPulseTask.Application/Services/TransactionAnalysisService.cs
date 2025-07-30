using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Application.Services;


public class TransactionAnalysisService : ITransactionAnalysisService
{
    private readonly IContractDateParser _contractDateParser;

    public TransactionAnalysisService(IContractDateParser contractDateParser)
    {
        _contractDateParser = contractDateParser ?? throw new ArgumentNullException(nameof(contractDateParser));
    }

    public IEnumerable<ContractAnalysisResult> AnalyzeTransactions(
        IEnumerable<TransactionHistoryGipData> transactions)
    {
        if (transactions == null)
            throw new ArgumentNullException(nameof(transactions));

        var groupedTransactions = transactions.GroupBy(t => t.ContractName);
        var results = new List<ContractAnalysisResult>();

        foreach (var group in groupedTransactions)
        {
            var contractName = group.Key;
            var contractTransactions = group.ToList();
            var totalAmount = contractTransactions.Sum(t => (t.Price * t.Quantity) / 10m);
            var totalQuantity = contractTransactions.Sum(t => t.Quantity / 10m);
            var weightedAveragePrice = totalQuantity != 0 ? totalAmount / totalQuantity : 0;
            var contractDateTime = _contractDateParser.ParseContractDateTime(contractName);

            results.Add(new ContractAnalysisResult(
                contractName,
                contractDateTime,
                totalAmount,
                totalQuantity,
                weightedAveragePrice,
                contractTransactions.Count));
        }

        return results.OrderBy(r => r.ContractDateTime);
    }
}