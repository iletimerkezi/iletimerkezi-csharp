using IletiMerkezi.Http;
using IletiMerkezi.Services;
using System.Text.Json;

namespace IletiMerkezi
{
    public class IletiMerkeziClient
    {
        private readonly IHttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _apiHash;
        private readonly string _defaultSender;

        public IletiMerkeziClient(string apiKey, string apiHash, string defaultSender = null)
        {
            _apiKey = apiKey;
            _apiHash = apiHash;
            _defaultSender = defaultSender;
            _httpClient = new DefaultHttpClient();
        }

        public AccountService Account() => new AccountService(_httpClient, _apiKey, _apiHash);
        public BlacklistService Blacklist() => new BlacklistService(_httpClient, _apiKey, _apiHash);
        public ReportService Reports() => new ReportService(_httpClient, _apiKey, _apiHash);
        public SenderService Senders() => new SenderService(_httpClient, _apiKey, _apiHash);
        public SummaryService Summary() => new SummaryService(_httpClient, _apiKey, _apiHash);
        public SmsService Sms() => new SmsService(_httpClient, _apiKey, _apiHash, _defaultSender);
        public WebhookService Webhook() => new WebhookService();

        public string Debug()
        {
            var debugInfo = new
            {
                payload = JsonSerializer.Deserialize<object>(_httpClient.GetLastPayload()),
                response = JsonSerializer.Deserialize<object>(_httpClient.GetLastResponse()),
                status = _httpClient.GetLastStatusCode()
            };

            return JsonSerializer.Serialize(debugInfo, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
        }
    }
}