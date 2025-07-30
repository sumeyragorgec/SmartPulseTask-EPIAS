namespace SmartPulseTask.Domain.Entities;


public class TransactionHistoryGipData
{
    public long Id { get; private set; }
    public DateTime Date { get; private set; }
    public string ContractName { get; private set; }
    public decimal Price { get; private set; }
    public decimal Quantity { get; private set; }

    public TransactionHistoryGipData(long id, DateTime date, string contractName, decimal price, decimal quantity)
    {
        if (string.IsNullOrWhiteSpace(contractName))
            throw new ArgumentException("Contract name cannot be null or empty", nameof(contractName));

        if (price < 0)
            throw new ArgumentException("Price cannot be negative", nameof(price));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        Id = id;
        Date = date;
        ContractName = contractName;
        Price = price;
        Quantity = quantity;
    }
}