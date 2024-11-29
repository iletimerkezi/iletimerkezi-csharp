using System;
using System.Text.Json;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class BlacklistService : BaseService
    {
        public BlacklistService(IHttpClient httpClient, string apiKey, string apiHash) 
            : base(httpClient, apiKey, apiHash)
        {
        }

        public async Task<BlacklistResponse> ListAsync(DateTime? startDate = null, DateTime? endDate = null, int page = 1, int rowCount = 1000)
        {
            var payload = new
            {
                request = new
                {
                    authentication = new
                    {
                        key = _apiKey,
                        hash = _apiHash
                    },
                    blacklist = new
                    {
                        filter = startDate.HasValue || endDate.HasValue ? new
                        {
                            start = startDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                            end = endDate?.ToString("yyyy-MM-dd HH:mm:ss")
                        } : null,
                        page,
                        rowCount = System.Math.Min(rowCount, 1000)
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("get-blacklist/json", payload);
            var responseBody = httpResponse.Body;

            var blacklistResponse = JsonSerializer.Deserialize<BlacklistResponse>(responseBody, _jsonSerializerOptions);
            return blacklistResponse;
        }

        public async Task<BlacklistResponse> CreateAsync(string number)
        {
            var payload = new
            {
                request = new
                {
                    authentication = new
                    {
                        key = _apiKey,
                        hash = _apiHash
                    },
                    blacklist = new
                    {
                        number
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("add-blacklist/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i BlacklistResponse nesnesine dönüştür
            var blacklistResponse = JsonSerializer.Deserialize<BlacklistResponse>(responseBody, _jsonSerializerOptions);

            return blacklistResponse;
        }

        public async Task<BlacklistResponse> DeleteAsync(string number)
        {
            var payload = new
            {
                request = new
                {
                    authentication = new
                    {
                        key = _apiKey,
                        hash = _apiHash
                    },
                    blacklist = new
                    {
                        number
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("delete-blacklist/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i BlacklistResponse nesnesine dönüştür
            var blacklistResponse = JsonSerializer.Deserialize<BlacklistResponse>(responseBody, _jsonSerializerOptions);

            return blacklistResponse;
        }
    }
}