using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi;

namespace IletiMerkezi.Tests.Mocks
{
    public class MockHttpClient : IHttpClient
    {
        private readonly string _responseToReturn;
        private string _lastPayload;
        private string _lastUserAgent;
        private int _statusCode;

        public MockHttpClient(string responseToReturn, int statusCode = 200)
        {
            _responseToReturn = responseToReturn;
            _statusCode = statusCode;
            _lastUserAgent = Constants.UserAgent;
        }

        public Task<HttpResponse> PostAsync(string url, object payload)
        {
            _lastPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            
            return Task.FromResult(new HttpResponse 
            { 
                Body = _responseToReturn,
                StatusCode = _statusCode
            });
        }

        public string GetLastPayload() => _lastPayload;
        public string GetLastUserAgent() => _lastUserAgent;
        public string GetLastResponse() => _responseToReturn;
        public int GetLastStatusCode() => _statusCode;
    }
}