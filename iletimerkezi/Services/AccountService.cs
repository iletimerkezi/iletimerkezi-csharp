using System;
using System.Text.Json;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class AccountService : BaseService
    {
        public AccountService(IHttpClient httpClient, string apiKey, string apiHash) 
            : base(httpClient, apiKey, apiHash)
        {
        }

        public async Task<AccountResponse> GetBalanceAsync()
        {
            var payload = CreateAuthenticationPayload();
            
            var httpResponse = await _httpClient.PostAsync("get-balance/json", payload);
            
            var accountResponse = JsonSerializer.Deserialize<AccountResponse>(httpResponse.Body, _jsonSerializerOptions);

            return accountResponse;
        }
    }
} 