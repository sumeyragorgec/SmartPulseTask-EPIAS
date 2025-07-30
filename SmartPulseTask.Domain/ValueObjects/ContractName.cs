namespace SmartPulseTask.Domain.ValueObjects;


public class ContractName
{
    public string Value { get; private set; }

    public ContractName(string value)
    {
  
        Value = value;
    }


    public DateTime ParseDateTime()
    {
        if (Value.Length < 10 || !Value.StartsWith("PH"))
            return DateTime.MinValue;

        try
        {
            var dateTimeStr = Value.Substring(2, 8);
            var year = 2000 + int.Parse(dateTimeStr.Substring(0, 2));
            var month = int.Parse(dateTimeStr.Substring(2, 2));
            var day = int.Parse(dateTimeStr.Substring(4, 2));
            var hour = int.Parse(dateTimeStr.Substring(6, 2));

            return new DateTime(year, month, day, hour, 0, 0);
        }
        catch
        {
            return DateTime.MinValue;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ContractName other) return false;
        return Value == other.Value;
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(ContractName contractName)
    {
        return contractName.Value;
    }

    public static implicit operator ContractName(string value)
    {
        return new ContractName(value);
    }
}
