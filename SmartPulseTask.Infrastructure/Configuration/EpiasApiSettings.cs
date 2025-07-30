namespace SmartPulseTask.Infrastructure.Configuration;

public class EpiasApiSettings
{
    public const string SectionName = "EpiasApi";

    public string AuthenticationEndpoint { get; set; } = "https://giris.epias.com.tr/cas/v1/tickets";
    public string TransactionHistoryEndpoint { get; set; } = "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history";
    public int TgtValidityHours { get; set; } = 2;
    public int HttpTimeoutSeconds { get; set; } = 30;
}
