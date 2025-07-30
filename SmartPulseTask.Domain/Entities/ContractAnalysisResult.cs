namespace SmartPulseTask.Domain.Entities;
public class ContractAnalysisResult
{
    public string ContractName { get; private set; }
    public DateTime ContractDateTime { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal TotalQuantity { get; private set; }
    public decimal WeightedAveragePrice { get; private set; }
    public int TransactionCount { get; private set; }

    public ContractAnalysisResult(
        string contractName,
        DateTime contractDateTime,
        decimal totalAmount,
        decimal totalQuantity,
        decimal weightedAveragePrice,
        int transactionCount)
    {
        ContractName = contractName ?? throw new ArgumentNullException(nameof(contractName));
        ContractDateTime = contractDateTime;
        TotalAmount = totalAmount;
        TotalQuantity = totalQuantity;
        WeightedAveragePrice = weightedAveragePrice;
        TransactionCount = transactionCount;
    }
}
