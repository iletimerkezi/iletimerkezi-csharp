using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IletiMerkezi.Http
{
    public class DefaultHttpClient : IHttpClient
    {
        private readonly HttpClient _client;
        private string _lastPayload;
        private string _lastResponse;
        private int _lastStatusCode;
        private const string BaseUrl = "https://api.iletimerkezi.com/v1/";

        public DefaultHttpClient()
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("User-Agent", Constants.UserAgent);
            _client.BaseAddress = new System.Uri(BaseUrl);
        }

        public async Task<HttpResponse> PostAsync(string url, object payload)
        {
            var json = JsonSerializer.Serialize(payload);
            _lastPayload = json;

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, content);
            
            var body = await response.Content.ReadAsStringAsync();
            
            _lastResponse = body;
            _lastStatusCode = (int)response.StatusCode;

            return new HttpResponse
            {
                Body = body,
                StatusCode = _lastStatusCode
            };
        }

        public string GetLastPayload()
        {
            return _lastPayload;
        }

        public string GetLastResponse()
        {
            return _lastResponse;
        }

        public int GetLastStatusCode()
        {
            return _lastStatusCode;
        }
    }

    public interface IHttpClient
    {
        Task<HttpResponse> PostAsync(string url, object payload);
        string GetLastPayload();
        string GetLastResponse();
        int GetLastStatusCode();
    }

    public class HttpResponse
    {
        public string Body { get; set; }
        public int StatusCode { get; set; }
    }
}