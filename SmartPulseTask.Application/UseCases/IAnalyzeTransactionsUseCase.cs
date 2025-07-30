using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public interface IAnalyzeTransactionsUseCase
{
    Task<IEnumerable<ContractAnalysisResult>> ExecuteAsync(
        UserCredentials credentials,
        DateRange dateRange);
}
