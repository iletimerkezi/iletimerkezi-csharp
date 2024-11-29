using IletiMerkezi.Http;
using System.Text.Json;

namespace IletiMerkezi.Services
{
    public abstract class BaseService
    {
        protected readonly IHttpClient _httpClient;
        protected readonly JsonSerializerOptions _jsonSerializerOptions;
        protected readonly string _apiKey;
        protected readonly string _apiHash;

        protected BaseService(IHttpClient httpClient, string apiKey, string apiHash)
        {
            _httpClient = httpClient;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _apiKey = apiKey;
            _apiHash = apiHash;
        }

        protected object CreateAuthenticationPayload()
        {
            return new
            {
                request = new
                {
                    authentication = new
                    {
                        key = _apiKey,
                        hash = _apiHash
                    }
                }
            };
        }
    }
} 