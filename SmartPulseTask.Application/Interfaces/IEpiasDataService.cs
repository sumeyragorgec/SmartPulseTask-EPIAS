using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.Interfaces;
public interface IEpiasDataService
{
    Task<IEnumerable<TransactionHistoryGipData>> GetTransactionHistoryAsync(
        TgtToken tgtToken,
        DateRange dateRange);
}