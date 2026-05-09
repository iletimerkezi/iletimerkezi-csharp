using System;
using System.Text.Json;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class SummaryService
    {
        private readonly IHttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiHash;
        private string _lastStartDate;
        private string _lastEndDate;
        private int _lastPage;

        public SummaryService(IHttpClient httpClient, string apiKey, string apiHash)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
            _apiHash = apiHash;
        }

        private static void ValidateDateFormat(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"'{paramName}' boş olamaz.");

            if (!DateTime.TryParseExact(value, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out _))
                throw new ArgumentException(
                    $"Geçersiz '{paramName}' formatı: '{value}'. Beklenen format: YYYY-AA-GG (örn: 2026-12-25).");
        }

        private static void ValidateDateRange(string startDate, string endDate)
        {
            var start = DateTime.ParseExact(startDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);
            var end = DateTime.ParseExact(endDate, "yyyy-MM-dd",
                System.Globalization.CultureInfo.InvariantCulture);

            if (end < start)
                throw new ArgumentException($"'endDate' ({endDate}), 'startDate' ({startDate}) tarihinden önce olamaz.");

            if ((end - start).TotalDays > 10)
                throw new ArgumentException(
                    $"'startDate' ile 'endDate' arasındaki fark en fazla 10 gün olabilir. Girilen aralık: {(end - start).TotalDays} gün.");
        }

        public async Task<SummaryResponse> ListAsync(string startDate, string endDate, int page = 1)
        {
            ValidateDateFormat(startDate, nameof(startDate));
            ValidateDateFormat(endDate, nameof(endDate));
            ValidateDateRange(startDate, endDate);

            _lastStartDate = startDate;
            _lastEndDate = endDate;
            _lastPage = page;

            var payload = new
            {
                request = new
                {
                    authentication = new
                    {
                        key = _apiKey,
                        hash = _apiHash
                    },
                    filter = new
                    {
                        start = startDate,
                        end = endDate,
                        page = page
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("get-reports/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i SummaryResponse nesnesine dönüştür
            var summaryResponse = JsonSerializer.Deserialize<SummaryResponse>(responseBody);

            return summaryResponse;
        }

        public async Task<SummaryResponse> NextAsync()
        {
            if (string.IsNullOrEmpty(_lastStartDate) || string.IsNullOrEmpty(_lastEndDate) || _lastPage == 0)
            {
                throw new System.InvalidOperationException("No previous report request found. Call ListAsync first.");
            }

            return await ListAsync(_lastStartDate, _lastEndDate, _lastPage + 1);
        }
    }
}