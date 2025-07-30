namespace SmartPulseTask.Web.Models;

public class DashboardStatisticsViewModel
{
    public int TotalTransactions { get; set; }
    public int UniqueContracts { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQuantity { get; set; }
    public DateTime? LastAnalysisDate { get; set; }
    public string MostActiveContract { get; set; } = string.Empty;
}
