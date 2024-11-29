using System.Text.Json;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class SenderService : BaseService
    {
        public SenderService(IHttpClient httpClient, string apiKey, string apiHash) 
            : base(httpClient, apiKey, apiHash)
        {
        }

        public async Task<SenderResponse> ListAsync()
        {
            var payload = CreateAuthenticationPayload();
            
            var httpResponse = await _httpClient.PostAsync("get-sender/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i SenderResponse nesnesine dönüştür
            var senderResponse = JsonSerializer.Deserialize<SenderResponse>(responseBody);

            return senderResponse;
        }
    }
}