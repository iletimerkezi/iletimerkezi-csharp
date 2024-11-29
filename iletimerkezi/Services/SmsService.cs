using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IletiMerkezi.Http;
using IletiMerkezi.Models;
using System.Text.Json;

namespace IletiMerkezi.Services
{
    public class SmsService : BaseService
    {
        private readonly string _defaultSender;
        private string _sendDateTime = "";
        private int _iys = 1;
        private string _iysList = "BIREYSEL";

        public SmsService(IHttpClient httpClient, string apiKey, string apiHash, string defaultSender) 
            : base(httpClient, apiKey, apiHash)
        {
            _defaultSender = defaultSender;
        }

        public SmsService Schedule(string sendDateTime)
        {
            _sendDateTime = sendDateTime;
            return this;
        }

        public SmsService EnableIysConsent()
        {
            _iys = 1;
            return this;
        }

        public SmsService DisableIysConsent()
        {
            _iys = 0;
            return this;
        }

        public SmsService SetIysList(string iysList)
        {
            _iysList = iysList;
            return this;
        }

        public async Task<SmsResponse> SendAsync(string recipient, string message, string sender = null)
        {
            return await SendAsync(new[] { recipient }, message, sender);
        }

        public async Task<SmsResponse> SendAsync(IEnumerable<string> recipients, string message, string sender = null)
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
                    order = new
                    {
                        sender = sender ?? _defaultSender,
                        sendDateTime = _sendDateTime,
                        iys = _iys,
                        iysList = _iysList,
                        message = new
                        {
                            text = message,
                            receipents = new
                            {
                                number = recipients
                            }
                        }
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("send-sms/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i SmsResponse nesnesine dönüştür
            var smsResponse = JsonSerializer.Deserialize<SmsResponse>(responseBody);

            return smsResponse;
        }

        public async Task<SmsResponse> SendBulkAsync(IDictionary<string, string> recipientMessages, string sender = null)
        {
            var messages = new List<object>();
            foreach (var kvp in recipientMessages)
            {
                messages.Add(new
                {
                    text = kvp.Value,
                    receipents = new
                    {
                        number = new[] { kvp.Key }
                    }
                });
            }

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
                        sender = sender ?? _defaultSender,
                        sendDateTime = _sendDateTime,
                        iys = _iys,
                        iysList = _iysList,
                        message = messages
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("send-sms/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i SmsResponse nesnesine dönüştür
            var smsResponse = JsonSerializer.Deserialize<SmsResponse>(responseBody);

            return smsResponse;
        }

        public async Task<SmsResponse> CancelAsync(int orderId)
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
                    order = new
                    {
                        id = orderId
                    }
                }
            };

            var httpResponse = await _httpClient.PostAsync("cancel-order/json", payload);
            var responseBody = httpResponse.Body;

            // JSON string'i SmsResponse nesnesine dönüştür
            var smsResponse = JsonSerializer.Deserialize<SmsResponse>(responseBody);

            return smsResponse;
        }
    }
}