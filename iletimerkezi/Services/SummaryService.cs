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

        public async Task<SummaryResponse> ListAsync(string startDate, string endDate, int page = 1)
        {
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