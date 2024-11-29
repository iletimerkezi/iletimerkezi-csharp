using System.Text.Json;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class ReportService : BaseService
    {
        private int? _lastOrderId;
        private int _lastPage;

        public ReportService(IHttpClient httpClient, string apiKey, string apiHash) 
            : base(httpClient, apiKey, apiHash)
        {
        }

        public async Task<ReportResponse> GetAsync(int orderId, int page = 1, int rowCount = 1000)
        {
            _lastOrderId = orderId;
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
                    order = new
                    {
                        id = orderId,
                        page = page,
                        rowCount = System.Math.Min(rowCount, 1000)
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("get-report/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i ReportResponse nesnesine dönüştür
            var reportResponse = JsonSerializer.Deserialize<ReportResponse>(responseBody);

            return reportResponse;
        }

        public async Task<ReportResponse> NextAsync()
        {
            if (!_lastOrderId.HasValue)
            {
                throw new System.InvalidOperationException("No previous report request found. Call GetAsync first.");
            }

            return await GetAsync(_lastOrderId.Value, _lastPage + 1);
        }
    }
}