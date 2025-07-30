namespace SmartPulseTask.Infrastructure.Configuration;

public class CacheSettings
{
    public const string SectionName = "Cache";

    public int TgtTokenExpirationMinutes { get; set; } = 120; 
    public int DefaultCacheExpirationMinutes { get; set; } = 60;
}
