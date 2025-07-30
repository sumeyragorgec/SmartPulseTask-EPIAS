using SmartPulseTask.Application.Interfaces;

namespace SmartPulseTask.Infrastructure.Services;

public class ContractDateParser : IContractDateParser
{
    public DateTime ParseContractDateTime(string contractName)
    {
        if (string.IsNullOrWhiteSpace(contractName) ||
            contractName.Length < 10 ||
            !contractName.StartsWith("PH"))
        {
            return DateTime.MinValue;
        }

        try
        {
            // PHYYMMDDSS 
            var dateTimeStr = contractName.Substring(2, 8);
            var year = 2000 + int.Parse(dateTimeStr.Substring(0, 2));
            var month = int.Parse(dateTimeStr.Substring(2, 2));
            var day = int.Parse(dateTimeStr.Substring(4, 2));
            var hour = int.Parse(dateTimeStr.Substring(6, 2));

            return new DateTime(year, month, day, hour, 0, 0);
        }
        catch (Exception)
        {
            return DateTime.MinValue;
        }
    }
}
