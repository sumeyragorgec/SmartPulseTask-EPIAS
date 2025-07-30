using Microsoft.Extensions.Logging;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;
using System.Net;
using System.Text.RegularExpressions;

namespace SmartPulseTask.Infrastructure.Services;

public class EpiasAuthenticationService : IEpiasAuthenticationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EpiasAuthenticationService> _logger;
    private const string TGT_ENDPOINT = "https://giris.epias.com.tr/cas/v1/tickets";


    public EpiasAuthenticationService(
        HttpClient httpClient,
        ILogger<EpiasAuthenticationService> logger)
    {
        _httpClient = httpClient ;
        _logger = logger;
    }

    public async Task<TgtToken> GetTgtTokenAsync(UserCredentials credentials)
    {
        if (credentials == null)
            throw new ArgumentNullException(nameof(credentials));

        try
        { 
            var formData = new List<KeyValuePair<string, string>>
            {
                new("username", credentials.Username),
                new("password", credentials.Password)
            };

            var content = new FormUrlEncodedContent(formData);
             

            var response = await _httpClient.PostAsync(TGT_ENDPOINT, content);

            if (response.IsSuccessStatusCode)
            {
                var htmlResponse = await response.Content.ReadAsStringAsync();
                 
                 
                var tgtToken = ExtractTgtTokenFromHtml(htmlResponse);

                if (string.IsNullOrWhiteSpace(tgtToken))
                {
                    throw new HttpRequestException("EPİAŞ'tan TGT token parse edilemedi. HTML formatı beklenmedik.");
                }
                 
                var validityDuration = TimeSpan.FromHours(2);
                var token = new TgtToken(tgtToken, validityDuration);
                 

                return token;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync(); 
                 
                var errorMessage = response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => "Kullanıcı adı veya şifre hatalı",
                    HttpStatusCode.Forbidden => "Bu hesapla giriş yapma yetkiniz bulunmamaktadır",
                    HttpStatusCode.NotFound => "EPİAŞ authentication servisi bulunamadı",
                    HttpStatusCode.InternalServerError => "EPİAŞ sunucu hatası",
                    HttpStatusCode.BadGateway => "EPİAŞ servisi geçici olarak erişilebilir değil",
                    HttpStatusCode.ServiceUnavailable => "EPİAŞ servisi bakımda",
                    HttpStatusCode.GatewayTimeout => "EPİAŞ servisi zaman aşımına uğradı",
                    _ => $"EPİAŞ servisinden beklenmeyen yanıt: {response.StatusCode}"
                };

                throw new HttpRequestException($"{errorMessage} (HTTP {(int)response.StatusCode})");
            }
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        { 
            throw new HttpRequestException("EPİAŞ servisine bağlanırken zaman aşımına uğradı. ");
        }
        catch (HttpRequestException)
        { 
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("EPİAŞ servisine bağlanırken beklenmeyen bir hata oluştu.", ex);
        }
    }

    private string ExtractTgtTokenFromHtml(string htmlResponse)
    {
        if (string.IsNullOrWhiteSpace(htmlResponse))
            return string.Empty;

        try
        {
            var actionPattern = @"action=""https://giris\.epias\.com\.tr/cas/v1/tickets/(TGT-[^""]+)""";
            var match = Regex.Match(htmlResponse, actionPattern, RegexOptions.IgnoreCase);

            if (match.Success && match.Groups.Count > 1)
            {
                var tgtToken = match.Groups[1].Value;
                return tgtToken;
            }
            return string.Empty;
        }
        catch (Exception ex)
        { 
            return string.Empty;
        }
    }
}
