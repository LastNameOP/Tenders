using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using Testovoe.Models;

public class TenderClientService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<TenderClientService> _logger;

    public TenderClientService(HttpClient httpClient, IConfiguration config, ILogger<TenderClientService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;

        var apiUrl = _config["APISettings:url"] ?? throw new InvalidOperationException("API URL not configured");
        _httpClient.BaseAddress = new Uri(apiUrl);
    }

    public async Task<IEnumerable<TenderJson>> GetTendersAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("");
            response.EnsureSuccessStatusCode();

            var tenders = await response.Content.ReadFromJsonAsync<IEnumerable<TenderJson>>();
            return tenders ?? Enumerable.Empty<TenderJson>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Ошибка при получении данных тендеров");
            return Enumerable.Empty<TenderJson>();
        }
    }
}

