using SmartPulseTask.Domain.Entities;

namespace SmartPulseTask.Infrastructure.Services;

public interface IChartDataService
{
    object GetHourlyAnalysis(IEnumerable<TransactionHistoryGipData> transactions);
    object GetContractAnalysis(IEnumerable<TransactionHistoryGipData> transactions);
    object GetPriceTrend(IEnumerable<TransactionHistoryGipData> transactions);
    object GetVolumeAnalysis(IEnumerable<TransactionHistoryGipData> transactions);
    object GetSummaryStats(IEnumerable<TransactionHistoryGipData> transactions);
}

public class ChartDataService : IChartDataService
{
    public object GetHourlyAnalysis(IEnumerable<TransactionHistoryGipData> transactions)
    {
        return transactions
            .GroupBy(t => t.Date.Hour)
            .Select(g => new
            {
                hour = $"{g.Key:00}:00",
                totalVolume = g.Sum(t => t.Quantity),
                totalValue = g.Sum(t => t.Price * t.Quantity),
                transactionCount = g.Count(),
                avgPrice = g.Sum(t => t.Price * t.Quantity) / g.Sum(t => t.Quantity), // Weighted average
                minPrice = g.Min(t => t.Price),
                maxPrice = g.Max(t => t.Price)
            })
            .OrderBy(x => x.hour)
            .ToList();
    }

    public object GetContractAnalysis(IEnumerable<TransactionHistoryGipData> transactions)
    {
        return transactions
            .GroupBy(t => t.ContractName)
            .Select(g => new
            {
                name = g.Key,
                totalVolume = g.Sum(t => t.Quantity),
                totalValue = g.Sum(t => t.Price * t.Quantity),
                transactionCount = g.Count(),
                avgPrice = g.Sum(t => t.Price * t.Quantity) / g.Sum(t => t.Quantity)
            })
            .OrderByDescending(x => x.totalValue)
            .Take(24)
            .ToList();
    }

    public object GetPriceTrend(IEnumerable<TransactionHistoryGipData> transactions)
    {
        return transactions
            .OrderBy(t => t.Date)
            .Take(100) 
            .Select((t, index) => new
            {
                time = t.Date.ToString("HH:mm"),
                price = t.Price,
                volume = t.Quantity,
                value = t.Price * t.Quantity,
                contract = t.ContractName,
                sequence = index + 1
            })
            .ToList();
    }

    public object GetVolumeAnalysis(IEnumerable<TransactionHistoryGipData> transactions)
    {
        return transactions
            .OrderBy(t => t.Date)
            .Take(100)
            .Select((t, index) => new
            {
                time = t.Date.ToString("HH:mm"),
                volume = t.Quantity,
                cumulativeVolume = transactions
                    .Where(x => x.Date <= t.Date)
                    .Sum(x => x.Quantity),
                sequence = index + 1
            })
            .ToList();
    }

    public object GetSummaryStats(IEnumerable<TransactionHistoryGipData> transactions)
    {
        var transactionList = transactions.ToList();
        var totalVolume = transactionList.Sum(t => t.Quantity);
        var totalValue = transactionList.Sum(t => t.Price * t.Quantity);

        return new
        {
            totalTransactions = transactionList.Count,
            totalVolume = totalVolume,
            totalValue = totalValue,
            avgPrice = totalValue / totalVolume,
            minPrice = transactionList.Min(t => t.Price),
            maxPrice = transactionList.Max(t => t.Price),
            uniqueContracts = transactionList.Select(t => t.ContractName).Distinct().Count(),
            dateRange = new
            {
                start = transactionList.Min(t => t.Date),
                end = transactionList.Max(t => t.Date)
            }
        };
    }
}
