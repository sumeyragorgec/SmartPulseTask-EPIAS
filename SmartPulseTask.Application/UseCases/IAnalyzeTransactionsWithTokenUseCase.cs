using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;

namespace SmartPulseTask.Application.UseCases;

public interface IAnalyzeTransactionsWithTokenUseCase
{
    Task<IEnumerable<ContractAnalysisResult>> ExecuteAsync(
        TgtToken tgtToken,
          DateRange dateRange);

}
