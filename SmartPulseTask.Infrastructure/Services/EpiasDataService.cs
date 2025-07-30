using Microsoft.Extensions.Logging;
using SmartPulseTask.Application.Interfaces;
using SmartPulseTask.Domain.Entities;
using SmartPulseTask.Domain.ValueObjects;
using System.Text;
using System.Text.Json;

namespace SmartPulseTask.Infrastructure.Services;

public class EpiasDataService : IEpiasDataService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EpiasDataService> _logger;
    private const string TRANSACTION_HISTORY_ENDPOINT = "https://seffaflik.epias.com.tr/electricity-service/v1/markets/idm/data/transaction-history";

    public EpiasDataService(
        HttpClient httpClient,
        ILogger<EpiasDataService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    private IEnumerable<TransactionHistoryGipData> ParseTransactionResponse(string jsonResponse)
    {
        try
        { 
            if (string.IsNullOrWhiteSpace(jsonResponse))
            { 
                return Enumerable.Empty<TransactionHistoryGipData>();
            }
             
            var contentPreview = jsonResponse.Length > 1000 ? jsonResponse.Substring(0, 1000) + "..." : jsonResponse;

            using var document = JsonDocument.Parse(jsonResponse);
            var root = document.RootElement;

 
            JsonElement dataArray;

             if (root.TryGetProperty("items", out dataArray))
            {
                 return ParseTransactionArray(dataArray, "items");
            }
            throw new InvalidOperationException("EPİAŞ API response formatı tanınmıyor - veri yapısı bulunamadı");
        }
        catch (JsonException ex)
        { 
            var problematicJson = jsonResponse?.Length > 1000 ? jsonResponse.Substring(0, 1000) + "..." : jsonResponse;
            throw new InvalidOperationException("EPİAŞ response JSON parse edilemedi", ex);
        }
        catch (Exception ex)
        { 
            throw;
        }
    }

    private IEnumerable<TransactionHistoryGipData> ParseTransactionArray(JsonElement arrayElement, string sourcePath)
    {
        var transactions = new List<TransactionHistoryGipData>();

        if (arrayElement.ValueKind != JsonValueKind.Array)
        {
             return transactions;
        }

        var arrayLength = arrayElement.GetArrayLength();

        if (arrayLength == 0)
        {
            return transactions;
        } 
        var firstElement = arrayElement[0]; 

        if (firstElement.ValueKind == JsonValueKind.Object)
        { 
            foreach (var prop in firstElement.EnumerateObject())
            {
                var valuePreview = prop.Value.ValueKind switch
                {
                    JsonValueKind.String => $"\"{prop.Value.GetString()}\"",
                    JsonValueKind.Number => prop.Value.ToString(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => "null",
                    _ => prop.Value.ValueKind.ToString()
                }; 
            }
        }
         
        const int batchSize = 1000;
        int processedCount = 0;

        for (int startIndex = 0; startIndex < arrayLength; startIndex += batchSize)
        {
            int endIndex = Math.Min(startIndex + batchSize, arrayLength); 
            for (int i = startIndex; i < endIndex; i++)
            {
                try
                {
                    var item = arrayElement[i];
                    var transaction = ParseSingleTransaction(item);
                    if (transaction != null)
                    {
                        transactions.Add(transaction);
                        processedCount++;
                    }
                } 
                catch (Exception)
                { 
                    continue;
                }
            }
        }

        return transactions;
    }

    private TransactionHistoryGipData? ParseSingleTransaction(JsonElement item)
    {
        if (item.ValueKind != JsonValueKind.Object)
            return null;

        try
        { 
            long id = 0;
            DateTime date = DateTime.MinValue;
            string contractName = string.Empty;
            decimal price = 0m;
            decimal quantity = 0m;
             
            if (item.TryGetProperty("id", out var idElement))
            {
                if (idElement.ValueKind == JsonValueKind.Number)
                {
                    id = idElement.GetInt64();
                }
            }
             
            if (item.TryGetProperty("date", out var dateElement))
            {
                var dateString = dateElement.GetString();
                if (!string.IsNullOrEmpty(dateString))
                {
                    if (DateTimeOffset.TryParse(dateString, out var dateOffset))
                    {
                        date = dateOffset.DateTime;
                    }
                    else if (DateTime.TryParse(dateString, out var parsedDate))
                    {
                        date = parsedDate;
                    }
                }
            } 
            if (item.TryGetProperty("contractName", out var contractElement))
            {
                contractName = contractElement.GetString() ?? string.Empty;
            }
             
            if (item.TryGetProperty("price", out var priceElement))
            {
                if (priceElement.ValueKind == JsonValueKind.Number)
                {
                    price = priceElement.GetDecimal();
                }
            }
             
            if (item.TryGetProperty("quantity", out var quantityElement))
            {
                if (quantityElement.ValueKind == JsonValueKind.Number)
                {
                    quantity = quantityElement.GetDecimal();
                }
            }
             
            if (string.IsNullOrEmpty(contractName))
            {
                return null;
            }

            if (price <= 0 || quantity <= 0)
            {
                return null;
            }

            return new TransactionHistoryGipData(id, date, contractName, price, quantity);
        }
        catch (Exception ex)
        { 
            return null;
        }
    }
    public async Task<IEnumerable<TransactionHistoryGipData>> GetTransactionHistoryAsync(
    TgtToken tgtToken,
    DateRange dateRange)
    {
        try
        { 
            var requestBody = new
            {
                startDate = dateRange.StartDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz"),
                endDate = dateRange.EndDate.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz")
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);

            
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("TGT", tgtToken.Value);
            _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

            _logger.LogInformation("Request Headers:");
            foreach (var header in _httpClient.DefaultRequestHeaders)
            {
                var headerValue = header.Key == "TGT" && header.Value.Any() ?
                    header.Value.First().Substring(0, Math.Min(30, header.Value.First().Length)) + "..." :
                    string.Join(", ", header.Value);
                _logger.LogInformation("  {Key}: {Value}", header.Key, headerValue);
            }
            var response = await _httpClient.PostAsync(TRANSACTION_HISTORY_ENDPOINT, content);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var transactions = ParseTransactionResponse(responseContent);
                return transactions;
            }
            else
            {
                _logger.LogError("Status: {StatusCode}", response.StatusCode);
                _logger.LogError("Reason: {ReasonPhrase}", response.ReasonPhrase);
                _logger.LogError("Content: {Content}", responseContent);

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    throw new HttpRequestException($"EPİAŞ API Bad Request - Detay: {responseContent}");
                }

                throw new HttpRequestException($"EPİAŞ API Error: {response.StatusCode} - {responseContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HttpRequestException in GetTransactionHistoryAsync");
            throw;
        }
        catch (Exception ex)
        {
            throw new HttpRequestException("Unexpected Exception in GetTransactionHistoryAsync", ex);
        }
    }
}