namespace SmartPulseTask.Web.Models; 

public class TgtTokenViewModel
{
    public string Value { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsExpired { get; set; }
    public TimeSpan TimeRemaining { get; set; }

    public string FormattedValue
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Value))
                return "---";

            if (Value.Length <= 30)
                return Value;

            var parts = Value.Split('-');
            if (parts.Length >= 3)
            {
                return $"{parts[0]}-{parts[1]}-{parts[2].Substring(0, Math.Min(8, parts[2].Length))}...";
            }
            return $"{Value.Substring(0, 20)}...{Value.Substring(Value.Length - 10)}";
        }
    }

    public string TokenId
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Value) || !Value.StartsWith("TGT-"))
                return "---";

            var parts = Value.Split('-');
            if (parts.Length >= 2)
                return parts[1];

            return "---";
        }
    }

    public string FormattedTimeRemaining
    {
        get
        {
            if (IsExpired)
                return "Süresi Dolmuş";

            if (TimeRemaining.TotalDays >= 1)
                return $"{TimeRemaining.Days} gün {TimeRemaining.Hours} saat";

            if (TimeRemaining.TotalHours >= 1)
                return $"{TimeRemaining.Hours} saat {TimeRemaining.Minutes} dakika";

            if (TimeRemaining.TotalMinutes >= 1)
                return $"{TimeRemaining.Minutes} dakika {TimeRemaining.Seconds} saniye";

            return $"{TimeRemaining.Seconds} saniye";
        }
    }

    public string Status => IsExpired ? "Süresi Dolmuş" : "Aktif";

    public string StatusClass => IsExpired ? "text-danger" : "text-success";

    public string BadgeClass => IsExpired ? "bg-danger" : "bg-success";
}