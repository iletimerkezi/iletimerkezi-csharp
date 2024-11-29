using System;
using System.Text.Json;
using IletiMerkezi.Models;

namespace IletiMerkezi.Services
{
    public class WebhookService
    {
        public WebhookReport Handle(string webhookData)
        {
            if (string.IsNullOrWhiteSpace(webhookData))
            {
                throw new ArgumentException("Webhook data cannot be null or empty");
            }

            return JsonSerializer.Deserialize<WebhookReport>(webhookData);
        }
    }
} 