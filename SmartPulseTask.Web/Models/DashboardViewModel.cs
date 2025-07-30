namespace SmartPulseTask.Web.Models;

public class DashboardViewModel
{
    public TgtTokenViewModel? CurrentToken { get; set; }
    public TransactionAnalysisViewModel TransactionAnalysis { get; set; } = new();
    public List<ContractAnalysisResultViewModel> RecentResults { get; set; } = new();
    public DashboardStatisticsViewModel Statistics { get; set; } = new();
}
