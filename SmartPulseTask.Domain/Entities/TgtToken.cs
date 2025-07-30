namespace SmartPulseTask.Domain.Entities;

public class TgtToken
{
    public string Value { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public TimeSpan ValidityDuration { get; private set; }

    public TgtToken(string value, TimeSpan validityDuration)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        ValidityDuration = validityDuration;
        CreatedAt = DateTime.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow - CreatedAt >= ValidityDuration;

    public DateTime ExpiresAt => CreatedAt.Add(ValidityDuration);
}
